using SunsetSystems.Entities.Characters;
using InsaneSystems.RTSSelection;
using SunsetSystems.Combat;
using SunsetSystems.Game;
using SunsetSystems.Utils.Input;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Linq;
using SunsetSystems.Party;
using SunsetSystems.Spellbook;
using SunsetSystems.Entities.Characters.Interfaces;
using SunsetSystems.Entities.Characters.Actions;
using SunsetSystems.Combat.Grid;
using SunsetSystems.Entities.Interfaces;
using Sirenix.OdinInspector;

namespace SunsetSystems.Input
{
    public class GameplayInputManager : SerializedMonoBehaviour
    {
        [Title("References")]
        [SerializeField]
        private Dictionary<GameState, IGameplayInputHandler> gameplayInputHandlers = new();
        private Vector2 mousePosition;
        private Collider lastHit;
        private const int raycastRange = 100;
        [SerializeField]
        private LayerMask _raycastTargetMask;
        [SerializeField]
        private float _followerStoppingDistance = 1.0f;

        private void OnEnable()
        {
            SunsetInputHandler.OnPrimaryAction += OnPrimaryAction;
            SunsetInputHandler.OnSecondaryAction += OnSecondaryAction;
            SunsetInputHandler.OnPointerPosition += OnPointerPosition;
        }

        private void OnDisable()
        {
            SunsetInputHandler.OnPrimaryAction -= OnPrimaryAction;
            SunsetInputHandler.OnSecondaryAction -= OnSecondaryAction;
            SunsetInputHandler.OnPointerPosition -= OnPointerPosition;
        }

        private static bool DoesAnyUIHitBlockRaycasts(List<RaycastResult> hits)
        {
            return hits.Any((hit) => { var cg = hit.gameObject.GetComponentInParent<CanvasGroup>(); return cg != null && cg.blocksRaycasts; });
        }

        private void OnPrimaryAction(InputAction.CallbackContext context)
        {
            if ((InputHelper.IsRaycastHittingUIObject(mousePosition, out List<RaycastResult> hits) && DoesAnyUIHitBlockRaycasts(hits)) is false)
            {
                if (gameplayInputHandlers.TryGetValue(GameManager.Instance.CurrentState, out IGameplayInputHandler handler))
                    handler.HandlePrimaryAction(context);
            }

        }

        private void OnSecondaryAction(InputAction.CallbackContext context)
        {
            if ((InputHelper.IsRaycastHittingUIObject(mousePosition, out List<RaycastResult> hits) && DoesAnyUIHitBlockRaycasts(hits)) is false)
            {
                if (gameplayInputHandlers.TryGetValue(GameManager.Instance.CurrentState, out IGameplayInputHandler handler))
                    handler.HandleSecondaryAction(context);
            }
        }

        private void OnPointerPosition(InputAction.CallbackContext context)
        {
            mousePosition = context.ReadValue<Vector2>();
            if (gameplayInputHandlers.TryGetValue(GameManager.Instance.CurrentState, out IGameplayInputHandler handler))
                handler.HandlePointerPosition(context);
        }

        private void HandleSecondaryAction()
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, raycastRange, _raycastTargetMask))
            {
                switch (GameManager.Instance.CurrentState)
                {
                    case GameState.Combat:
                        {
                            Debug.Log("Secondary action in combat.");
                            HandleCombatSecondaryAction(hit);
                            break;
                        }
                    case GameState.Exploration:
                        {
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
                ICreature mainCharacter = PartyManager.Instance.MainCharacter;
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
                    creatures.Add(PartyManager.Instance.MainCharacter);
                }
                if (PartyManager.Instance.ActiveParty.Count > 1)
                    creatures.AddRange(PartyManager.Instance.Companions);
                MoveCreaturesToPosition(creatures, hit.point);
            }
        }

        private void HandleCombatSecondaryAction(RaycastHit hit)
        {
            ActionBarUI.SelectedBarAction selectedBarAction = ActionBarUI.Instance.GetSelectedBarAction();
            switch (selectedBarAction.actionType)
            {
                case BarAction.MOVE:
                    if (!CombatManager.Instance.IsActiveActorPlayerControlled() || CombatManager.Instance.CurrentActiveActor.HasMoved)
                    {
                        Debug.Log($"Move bar action failed! Current actor {CombatManager.Instance.CurrentActiveActor.References.GameObject.name} is not player controlled or has already moved!");
                        return;
                    }
                    if (hit.collider.TryGetComponent(out IGridCell gridElement))
                    {
                        if (gridElement.IsFree)
                        {
                            Debug.Log($"Moving {CombatManager.Instance.CurrentActiveActor.References.GameObject.name} to grid element {gridElement}!");
                            //CombatManager.CurrentActiveActor.PerformAction(new Move(CombatManager.CurrentActiveActor, gridElement.WorldPosition, 0f));
                        }
                        else
                        {
                            Debug.Log($"Grid element {gridElement} is already occupied!");
                        }
                    }
                    else
                    {
                        Debug.Log($"Combat mouse click hit {hit.collider.gameObject.name} but found no GridElement!");
                    }
                    break;
                case BarAction.ATTACK:
                    if (!CombatManager.Instance.IsActiveActorPlayerControlled() || CombatManager.Instance.CurrentActiveActor.HasActed)
                        return;
                    ICombatant enemy = hit.collider.GetComponent<ICreature>().References.CombatBehaviour;
                    if (enemy != null)
                    {
                        if (enemy.Faction is Faction.Hostile && IsInRange(enemy))
                        {
                            Debug.Log($"{CombatManager.Instance.CurrentActiveActor} is attacking enemy {enemy}!");
                            throw new NotImplementedException();
                            //CombatManager.CurrentActiveActor.PerformAction(new Attack(enemy, CombatManager.CurrentActiveActor));
                        }
                    }
                    break;
                case BarAction.SELECT_TARGET:
                    if (!CombatManager.Instance.IsActiveActorPlayerControlled() || CombatManager.Instance.CurrentActiveActor.HasActed)
                        return;
                    ICombatant powerTarget = hit.collider.GetComponent<ICreature>().References.CombatBehaviour;
                    if (powerTarget != null)
                    {
                        if (VerifyTarget(powerTarget, SpellbookManager.RequiredTarget))
                        {
                            Debug.Log($"{CombatManager.Instance.CurrentActiveActor.References} is using power on enemy {powerTarget}!");
                            SpellbookManager.PowerTarget = powerTarget;
                        }
                    }
                    break;
                default:
                    Debug.LogError($"Invalid bar action in combat secondary action handler!");
                    break;
            }
        }

        private bool VerifyTarget(ICombatant target, Spellbook.Target requiredTarget)
        {
            return requiredTarget switch
            {
                Spellbook.Target.Self => target.Equals(CombatManager.Instance.CurrentActiveActor),
                Spellbook.Target.Friendly => target.Faction is Faction.PlayerControlled || target.Faction is Faction.Friendly,
                Spellbook.Target.Hostile => target.Faction is Faction.Hostile,
                Spellbook.Target.AOE_Friendly => throw new NotImplementedException(),
                Spellbook.Target.AOE_Hostile => throw new NotImplementedException(),
                _ => false,
            };
        }

        private static bool IsInRange(IEntity enemy)
        {
            int maxRange = CombatManager.Instance.CurrentActiveActor.CurrentWeapon.GetRangeData().maxRange;
            float distance = Vector3.Distance(CombatManager.Instance.CurrentActiveActor.References.Transform.position, enemy.References.Transform.position);
            return distance <= maxRange;
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
                switch (GameManager.Instance.CurrentState)
                {
                    case GameState.Exploration:
                        {
                            HandleExplorationPointerPosition(hit);
                            break;
                        }
                    case GameState.Combat:
                        {
                            //HandleCombatPointerPosition(hit);
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
                {
                    creature.PerformAction(new Move(creature, hit.position, stoppingDistance), true);
                }
            }
        }
    }

    public enum PlayerInputMode
    {
        Default, TargetSelection
    }
}
