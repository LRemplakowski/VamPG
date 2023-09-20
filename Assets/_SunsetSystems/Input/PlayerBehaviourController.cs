using SunsetSystems.Entities.Characters;
using InsaneSystems.RTSSelection;
using SunsetSystems.Combat;
using SunsetSystems.Game;
using SunsetSystems.Utils;
using SunsetSystems.Utils.Input;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Linq;
using SunsetSystems.Party;
using SunsetSystems.Entities;
using SunsetSystems.Spellbook;
using SunsetSystems.Entities.Characters.Interfaces;
using SunsetSystems.Entities.Characters.Actions;

namespace SunsetSystems.Input
{
    public class PlayerBehaviourController : Singleton<PlayerBehaviourController>
    {
        private Vector2 mousePosition;
        private Collider lastHit;
        private const int raycastRange = 100;
        [SerializeField]
        private LayerMask _raycastTargetMask;
        [SerializeField]
        private float _followerStoppingDistance = 1.0f;
        [SerializeField]
        private bool _useSelection;
        [SerializeField]
        private Selection _selection;

        private void OnEnable()
        {
            if (_selection)
                _selection.gameObject.SetActive(_useSelection);
            //PlayerInputHandler.OnPrimaryAction += OnPrimaryAction;
            PlayerInputHandler.OnSecondaryAction += OnSecondaryAction;
            PlayerInputHandler.OnPointerPosition += OnPointerPosition;
        }

        private void OnDisable()
        {
            //PlayerInputHandler.OnPrimaryAction -= OnPrimaryAction;
            PlayerInputHandler.OnSecondaryAction -= OnSecondaryAction;
            PlayerInputHandler.OnPointerPosition -= OnPointerPosition;
        }

        private void OnSecondaryAction(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            if (InputHelper.IsRaycastHittingUIObject(mousePosition, out List<RaycastResult> hits))
            {
                if (hits.Any(hit => hit.gameObject.GetComponentInParent<CanvasGroup>()?.blocksRaycasts ?? false))
                {
                    Debug.Log("Raycast hit UI object!");
                    return;
                }
            }
            HandleSecondaryAction();
        }

        private void HandleSecondaryAction()
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, raycastRange, _raycastTargetMask))
            {
                switch (GameManager.CurrentState)
                {
                    case GameState.Combat:
                        {
                            Debug.Log("Secondary action in combat.");
                            HandleCombatSecondaryAction(hit);
                            break;
                        }
                    case GameState.Exploration:
                        {
                            if (_useSelection)
                                HandleSelectionExplorationInput();
                            else
                                HandleNoSelectionExplorationInput();
                            break;
                        }
                    case GameState.Conversation:
                        {
                            break;
                        }
                    default:
                        break;
                }
            }
            else
            {
                Debug.LogWarning("Raycast failed on input secondary action!");
            }

            void HandleSelectionExplorationInput()
            {
                List<ISelectable> selectables = Selection.Instance.GetAllSelected();
                ICreature currentLead;
                if (selectables.Count > 0)
                {
                    currentLead = selectables[0].GetCreature();
                }
                else
                {
                    return;
                }
                if (hit.collider.gameObject.TryGetComponent(out IInteractable interactable))
                {
                    currentLead.PerformAction(new Interact(interactable, currentLead));
                }
                else
                {
                    MoveCurrentSelectionToPositions(hit);
                }
            }

            void HandleNoSelectionExplorationInput()
            {
                ICreature mainCharacter = PartyManager.MainCharacter;
                if (mainCharacter == null)
                    return;
                // Main Character should always take the lead since it's a first entry in ActiveParty list
                List<ICreature> creatures = new();
                IInteractable interactable = hit.collider.gameObject
                        .GetComponents<IInteractable>()?
                        .FirstOrDefault(interactable => (interactable as MonoBehaviour).enabled);
                if (interactable != null)
                {
                    mainCharacter.PerformAction(new Interact(interactable, mainCharacter));
                    creatures.Add(null);
                }
                else
                {
                    creatures.Add(PartyManager.MainCharacter);
                }
                if (PartyManager.ActiveParty.Count > 1)
                    creatures.AddRange(PartyManager.Companions);
                MoveCreaturesToPosition(creatures, hit.point);
            }
        }

        private void HandleCombatSecondaryAction(RaycastHit hit)
        {
            ActionBarUI.SelectedBarAction selectedBarAction = ActionBarUI.instance.GetSelectedBarAction();
            switch (selectedBarAction.actionType)
            {
                case BarAction.MOVE:
                    if (!CombatManager.IsActiveActorPlayerControlled() || CombatManager.CurrentActiveActor.GetComponentInChildren<CombatBehaviour>().HasMoved)
                    {
                        Debug.Log($"Move bar action failed! Current actor {CombatManager.CurrentActiveActor.References.CreatureData.DatabaseID} is not player controlled or has already moved!");
                        return;
                    }
                    if (hit.collider.TryGetComponent(out GridElement gridElement))
                    {
                        if (gridElement.Visited is not GridElement.Status.Occupied)
                        {
                            Debug.Log($"Moving {CombatManager.CurrentActiveActor.References.CreatureData.DatabaseID} to grid element {gridElement.gameObject.name}!");
                            CombatManager.CurrentActiveActor.PerformAction(new Move(CombatManager.CurrentActiveActor, gridElement.WorldPosition, 0f));
                        }
                        else
                        {
                            Debug.Log($"Grid element {gridElement.gameObject.name} is already occupied!");
                        }
                    }
                    else
                    {
                        Debug.Log($"Combat mouse click hit {hit.collider.gameObject.name} but found no GridElement!");
                    }
                    break;
                case BarAction.ATTACK:
                    if (!CombatManager.IsActiveActorPlayerControlled() || CombatManager.CurrentActiveActor.GetComponentInChildren<CombatBehaviour>().HasActed)
                        return;
                    Creature enemy = hit.collider.GetComponent<Creature>();
                    if (enemy)
                    {
                        if (enemy.References.CreatureData.Faction is Faction.Hostile && IsInRange(enemy))
                        {
                            Debug.Log($"{CombatManager.CurrentActiveActor.References.CreatureData.DatabaseID} is attacking enemy {enemy.References.CreatureData.DatabaseID}!");
                            CombatManager.CurrentActiveActor.PerformAction(new Attack(enemy, CombatManager.CurrentActiveActor));
                        }
                    }
                    break;
                case BarAction.SELECT_TARGET:
                    if (!CombatManager.IsActiveActorPlayerControlled() || CombatManager.CurrentActiveActor.GetComponentInChildren<CombatBehaviour>().HasActed)
                        return;
                    Creature powerTarget = hit.collider.GetComponent<Creature>();
                    if (powerTarget)
                    {
                        if (VerifyTarget(powerTarget, SpellbookManager.RequiredTarget))
                        {
                            Debug.Log($"{CombatManager.CurrentActiveActor.References.CreatureData.DatabaseID} is using power on enemy {powerTarget.References.CreatureData.DatabaseID}!");
                            SpellbookManager.PowerTarget = powerTarget;
                        }
                    }
                    break;
                default:
                    Debug.LogError($"Invalid bar action in combat secondary action handler!");
                    break;
            }
        }

        private bool VerifyTarget(Creature target, Spellbook.Target requiredTarget)
        {
            return requiredTarget switch
            {
                Spellbook.Target.Self => target.Equals(CombatManager.CurrentActiveActor),
                Spellbook.Target.Friendly => target.Faction is Faction.PlayerControlled || target.Faction is Faction.Friendly,
                Spellbook.Target.Hostile => target.References.CreatureData.Faction is Faction.Hostile,
                Spellbook.Target.AOE_Friendly => throw new NotImplementedException(),
                Spellbook.Target.AOE_Hostile => throw new NotImplementedException(),
                _ => false,
            };
        }

        private static bool IsInRange(Entity enemy)
        {
            int maxRange = CombatManager.CurrentActiveActor.CurrentWeapon.GetRangeData().maxRange;
            float distance = Vector3.Distance(CombatManager.CurrentActiveActor.transform.position, enemy.transform.position);
            return distance <= maxRange;
        }

        private void OnPointerPosition(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            mousePosition = context.ReadValue<Vector2>();
            if (InputHelper.IsRaycastHittingUIObject(mousePosition, out List<RaycastResult> hits))
            {
                if (hits.Any(hit => hit.gameObject.GetComponentInParent<CanvasGroup>()?.blocksRaycasts ?? false))
                {
                    return;
                }
            }
            HandlePointerPosition();
        }

        private void HandlePointerPosition()
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, raycastRange, _raycastTargetMask, QueryTriggerInteraction.Ignore))
            {
                if (lastHit == null)
                {
                    lastHit = hit.collider;
                }
                switch (GameManager.CurrentState)
                {
                    case GameState.Exploration:
                        {
                            HandleExplorationPointerPosition(hit);
                            break;
                        }
                    case GameState.Combat:
                        {
                            HandleCombatPointerPosition(hit);
                            break;
                        }
                    case GameState.Conversation:
                        {
                            break;
                        }
                    default:
                        break;
                }
            }

            void HandleExplorationPointerPosition(RaycastHit hit)
            {
                IInteractable interactable = null;
                if (lastHit != hit.collider)
                {
                    interactable = lastHit.gameObject
                        .GetComponents<IInteractable>()?
                        .FirstOrDefault(interactable => (interactable as MonoBehaviour).enabled);
                    if (interactable != null)
                    {
                        interactable.IsHoveredOver = false;
                        interactable = null;
                    }
                    lastHit = hit.collider;
                }
                interactable = lastHit.gameObject
                    .GetComponents<IInteractable>()?
                    .FirstOrDefault(interactable => (interactable as MonoBehaviour).enabled);
                if (interactable != null)
                {
                    interactable.IsHoveredOver = true;
                }
            }

            void HandleCombatPointerPosition(RaycastHit hit)
            {
                throw new NotImplementedException();
                //switch (selectedBarAction.actionType)
                //{
                //    case BarAction.MOVE:
                //        HandleMoveActionPointerPosition();
                //        break;
                //    case BarAction.ATTACK:
                //        HandleAttackActionPointerPosition();
                //        break;
                //}

                void HandleMoveActionPointerPosition()
                {
                    if (!CombatManager.IsActiveActorPlayerControlled())
                        return;
                    if (lastHit != hit.collider)
                    {
                        if (GridElement.IsInstance(lastHit.gameObject))
                        {
                            lastHit.gameObject.GetComponent<GridElement>().MouseOver = false;
                        }
                        lastHit = hit.collider;
                    }
                    if (GridElement.IsInstance(lastHit.gameObject))
                    {
                        lastHit.gameObject.GetComponent<GridElement>().MouseOver = true;
                    }
                }

                void HandleAttackActionPointerPosition()
                {
                    if (!CombatManager.IsActiveActorPlayerControlled() || CombatManager.CurrentActiveActor.GetComponentInChildren<CombatBehaviour>().HasActed)
                        return;
                    LineRenderer lineRenderer = CombatManager.CurrentActiveActor.GetComponentInChildren<CombatBehaviour>().LineRenderer;
                    if (lastHit != hit.collider)
                    {
                        lineRenderer.enabled = false;
                        lastHit = hit.collider;
                    }
                    ICreature creature = lastHit.GetComponent<ICreature>();
                    if (creature != null && creature.Faction is Faction.Hostile)
                    {
                        lineRenderer.positionCount = 2;
                        throw new NotImplementedException();
                        //lineRenderer.SetPosition(0, lineRenderer.transform.position);
                        //lineRenderer.SetPosition(1, creature.LineTarget.position);
                        //Color color = IsInRange(creature)
                        //    ? Color.green
                        //    : Color.red;
                        //lineRenderer.startColor = color;
                        //lineRenderer.endColor = color;
                        //lineRenderer.enabled = true;
                    }
                }
            }
        }

        private void MoveCurrentSelectionToPositions(RaycastHit hit)
        {
            List<ICreature> allSelected = Selection.Instance.GetAllSelected().Select(s => s.GetCreature()) as List<ICreature>;
            MoveCreaturesToPosition(allSelected, hit.point);
        }

        private void MoveCreaturesToPosition(List<ICreature> creatures, Vector3 samplingPoint)
        {
            float stoppingDistance = 0f;
            for (int i = 0; i < creatures.Count; i++)
            {
                NavMesh.SamplePosition(samplingPoint, out NavMeshHit hit, 2.0f, NavMesh.AllAreas);
                stoppingDistance += (i % 2) * _followerStoppingDistance;
                ICreature creature = creatures[i];
                if (creature != null)
                    creature.PerformAction(new Move(creature, hit.position, stoppingDistance));
            }
        }
    }

    public enum PlayerInputMode
    {
        Default, TargetSelection
    }
}
