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
using NaughtyAttributes;
using SunsetSystems.Party;
using SunsetSystems.Entities;
using SunsetSystems.Spellbook;

namespace SunsetSystems.Input
{
    public class PlayerBehaviourController : Singleton<PlayerBehaviourController>
    {
        private Vector2 mousePosition;
        private Collider lastHit;
        private const int raycastRange = 100;
        [SerializeField]
        private LayerMask defaultRaycastMask;
        [SerializeField]
        private float _followerStoppingDistance = 1.0f;
        [SerializeField]
        private bool _useSelection;
        [SerializeField]
        private Selection _selection;

        public static PlayerInputMode InputMode { get; set; }

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
            if (Physics.Raycast(ray, out RaycastHit hit, raycastRange, defaultRaycastMask))
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
                PlayerControlledCharacter currentLead;
                if (selectables.Count > 0)
                {
                    currentLead = selectables[0].GetCreature() as PlayerControlledCharacter;
                }
                else
                {
                    return;
                }
                if (hit.collider.gameObject.TryGetComponent(out IInteractable interactable))
                {
                    currentLead.ClearAllActions();
                    currentLead.InteractWith(interactable);
                }
                else
                {
                    MoveCurrentSelectionToPositions(hit);
                }
            }

            void HandleNoSelectionExplorationInput()
            {
                PlayerControlledCharacter mainCharacter = PartyManager.MainCharacter as PlayerControlledCharacter;
                if (mainCharacter == null)
                    return;
                // Main Character should always take the lead since it's a first entry in ActiveParty list
                List<Creature> creatures = new();
                if (hit.collider.gameObject.TryGetComponent(out IInteractable interactable))
                {
                    mainCharacter.ClearAllActions();
                    mainCharacter.InteractWith(interactable);
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
                    if (!CombatManager.IsActiveActorPlayerControlled() || CombatManager.CurrentActiveActor.CombatBehaviour.HasMoved && !DevMoveActorToPosition.InputOverride)
                    {
                        Debug.Log($"Move bar action failed! Current actor {CombatManager.CurrentActiveActor.Data.ID} is not player controlled or has already moved!");
                        return;
                    }
                    if (hit.collider.TryGetComponent(out GridElement gridElement))
                    {
                        if (gridElement.Visited is not GridElement.Status.Occupied)
                        {
                            Debug.Log($"Moving {CombatManager.CurrentActiveActor.Data.ID} to grid element {gridElement.gameObject.name}!");
                            CombatManager.CurrentActiveActor.Move(gridElement);
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
                    if (!CombatManager.IsActiveActorPlayerControlled() || CombatManager.CurrentActiveActor.CombatBehaviour.HasActed)
                        return;
                    Creature enemy = hit.collider.GetComponent<Creature>();
                    if (enemy)
                    {
                        if (enemy.Data.Faction is Faction.Hostile && IsInRange(enemy))
                        {
                            Debug.Log($"{CombatManager.CurrentActiveActor.Data.ID} is attacking enemy {enemy.Data.ID}!");
                            CombatManager.CurrentActiveActor.Attack(enemy);
                        }
                    }
                    break;
                case BarAction.SELECT_TARGET:
                    if (!CombatManager.IsActiveActorPlayerControlled() || CombatManager.CurrentActiveActor.CombatBehaviour.HasActed)
                        return;
                    Creature powerTarget = hit.collider.GetComponent<Creature>();
                    if (powerTarget)
                    {
                        if (VerifyTarget(powerTarget, SpellbookManager.RequiredTarget))
                        {
                            Debug.Log($"{CombatManager.CurrentActiveActor.Data.ID} is using power on enemy {powerTarget.Data.ID}!");
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
                Spellbook.Target.Friendly => target is PlayerControlledCharacter || target.Data.Faction is Faction.Friendly,
                Spellbook.Target.Hostile => target.Data.Faction is Faction.Hostile,
                Spellbook.Target.AOE_Friendly => throw new NotImplementedException(),
                Spellbook.Target.AOE_Hostile => throw new NotImplementedException(),
                _ => false,
            };
        }

        private static bool IsInRange(Entity enemy)
        {
            int maxRange = CombatManager.CurrentActiveActor.Data.Equipment.GetSelectedWeapon().GetRangeData().maxRange;
            float distance = Vector3.Distance(CombatManager.CurrentActiveActor.transform.position, enemy.transform.position);
            return distance <= maxRange;
        }

        private void OnPointerPosition(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            mousePosition = context.ReadValue<Vector2>();
            HandlePointerPosition();
        }

        private void HandlePointerPosition()
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, raycastRange, defaultRaycastMask, QueryTriggerInteraction.Ignore))
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
                if (lastHit != hit.collider)
                {
                    if (lastHit.gameObject.TryGetComponent(out IInteractable previousInteractable))
                    {
                        previousInteractable.IsHoveredOver = false;
                    }
                    lastHit = hit.collider;
                }
                if (lastHit.gameObject.TryGetComponent(out IInteractable currentInteractable))
                {
                    currentInteractable.IsHoveredOver = true;
                }
            }

            void HandleCombatPointerPosition(RaycastHit hit)
            {
                ActionBarUI.SelectedBarAction selectedBarAction = ActionBarUI.instance.GetSelectedBarAction();
                switch (selectedBarAction.actionType)
                {
                    case BarAction.MOVE:
                        HandleMoveActionPointerPosition();
                        break;
                    case BarAction.ATTACK:
                        HandleAttackActionPointerPosition();
                        break;
                }

                void HandleMoveActionPointerPosition()
                {
                    if (!CombatManager.IsActiveActorPlayerControlled() && !DevMoveActorToPosition.InputOverride)
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
                    if (!CombatManager.IsActiveActorPlayerControlled() || CombatManager.CurrentActiveActor.CombatBehaviour.HasActed)
                        return;
                    LineRenderer lineRenderer = CombatManager.CurrentActiveActor.CombatBehaviour.LineRenderer;
                    if (lastHit != hit.collider)
                    {
                        lineRenderer.enabled = false;
                        lastHit = hit.collider;
                    }
                    DefaultNPC creature = lastHit.GetComponent<DefaultNPC>();
                    if (creature)
                    {
                        lineRenderer.positionCount = 2;
                        lineRenderer.SetPosition(0, lineRenderer.transform.position);
                        lineRenderer.SetPosition(1, creature.LineTarget.position);
                        Color color = IsInRange(creature)
                            ? Color.green
                            : Color.red;
                        lineRenderer.startColor = color;
                        lineRenderer.endColor = color;
                        lineRenderer.enabled = true;
                    }
                }
            }
        }

        private void MoveCurrentSelectionToPositions(RaycastHit hit)
        {
            List<Creature> allSelected = Selection.Instance.GetAllSelected().Select(s => s.GetCreature()) as List<Creature>;
            MoveCreaturesToPosition(allSelected, hit.point);
        }

        private void MoveCreaturesToPosition(List<Creature> creatures, Vector3 samplingPoint)
        {
            float stoppingDistance = 0f;
            for (int i = 0; i < creatures.Count; i++)
            {
                NavMesh.SamplePosition(samplingPoint, out NavMeshHit hit, 2.0f, NavMesh.AllAreas);
                stoppingDistance += (i % 2) * _followerStoppingDistance;
                Creature creature = creatures[i];
                if (creature != null)
                    creature.Move(hit.position, stoppingDistance);
            }
        }
    }

    public enum PlayerInputMode
    {
        Default, TargetSelection
    }
}
