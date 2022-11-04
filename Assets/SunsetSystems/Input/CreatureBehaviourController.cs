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

namespace SunsetSystems.Input
{
    public class CreatureBehaviourController : Singleton<CreatureBehaviourController>
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
            if (InputHelper.IsRaycastHittingUIObject(mousePosition, out List<RaycastResult> hits))
            {
                if (hits.Any(hit => {
                        CanvasGroup group = hit.gameObject.GetComponent<CanvasGroup>();
                        if (group)
                            return group.blocksRaycasts;
                        return false;
                    }))
                {
                    Debug.Log("Raycast hit UI object!");
                    return;
                }
            }
            if (context.performed)
                HandleWorldRightClick();
        }

        private void HandleWorldRightClick()
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, raycastRange, defaultRaycastMask))
            {
                switch (GameManager.CurrentState)
                {
                    case GameState.Combat:
                        {
                            HandleCombatMouseClick(hit);
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
                        Debug.Log("Default click behaviour");
                        break;
                }
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
                PlayerControlledCharacter currentLead = PartyManager.MainCharacter as PlayerControlledCharacter;
            }
        }

        private void HandleCombatMouseClick(RaycastHit hit)
        {
            ActionBarUI.SelectedBarAction selectedBarAction = ActionBarUI.instance.GetSelectedBarAction();
            switch (selectedBarAction.actionType)
            {
                case BarAction.MOVE:
                    if (!CombatManager.IsActiveActorPlayerControlled() || CombatManager.CurrentActiveActor.CombatBehaviour.HasMoved && !DevMoveActorToPosition.InputOverride)
                        return;
                    GridElement gridElement = hit.collider.GetComponent<GridElement>();
                    if (gridElement)
                    {
                        if (gridElement.Visited != GridElement.Status.Occupied)
                        {
                            CombatManager.CurrentActiveActor.Move(gridElement);
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
                    DefaultNPC enemy = hit.collider.GetComponent<DefaultNPC>();
                    if (enemy)
                    {
                        if (enemy.Data.faction.Equals(Faction.Hostile) && IsInRange(enemy))
                        {
                            CombatManager.CurrentActiveActor.Attack(enemy);
                        }
                    }
                    break;
            }
        }

        private static bool IsInRange(DefaultNPC enemy)
        {
            return Vector3.Distance(CombatManager.CurrentActiveActor.transform.position, enemy.transform.position) <= CombatManager.CurrentActiveActor.StatsManager.GetWeaponMaxRange();
        }

        private void OnPointerPosition(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            mousePosition = context.ReadValue<Vector2>();
            HandleMousePosition();
        }

        private void HandleMousePosition()
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
                            HandleExplorationMousePosition(hit);
                            break;
                        }
                    case GameState.Combat:
                        {
                            HandleCombatMousePosition(hit);
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

            void HandleExplorationMousePosition(RaycastHit hit)
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

            void HandleCombatMousePosition(RaycastHit hit)
            {
                ActionBarUI.SelectedBarAction selectedBarAction = ActionBarUI.instance.GetSelectedBarAction();
                switch (selectedBarAction.actionType)
                {
                    case BarAction.MOVE:
                        HandleMoveActionMousePosition();
                        break;
                    case BarAction.ATTACK:
                        HandleAttackActionMousePosition();
                        break;
                }

                void HandleMoveActionMousePosition()
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

                void HandleAttackActionMousePosition()
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

        private void MoveSelectableToPosition(ISelectable selectable, Vector3 position, float stoppingDistance)
        {
            Creature creature = selectable.GetCreature();
            creature.Move(position, stoppingDistance);
        }

        private void MoveCurrentSelectionToPositions(RaycastHit hit)
        {
            Vector3 samplingPoint;
            List<ISelectable> allSelected = Selection.Instance.GetAllSelected();
            float stoppingDistance = 0f;
            for (int i = 0; i < allSelected.Count; i++)
            {
                samplingPoint = hit.point;
                NavMesh.SamplePosition(samplingPoint, out NavMeshHit navHit, 2.0f, NavMesh.AllAreas);
                stoppingDistance += (i % 2) * _followerStoppingDistance;
                MoveSelectableToPosition(allSelected[i], navHit.position, stoppingDistance);
            }
        }
    }
}
