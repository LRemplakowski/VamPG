using Entities;
using Entities.Characters;
using InsaneSystems.RTSSelection;
using SunsetSystems.Game;
using SunsetSystems.Utils;
using SunsetSystems.Utils.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace SunsetSystems.Input
{
    public class CreatureBehaviourController : Singleton<CreatureBehaviourController>
    {
        private Vector2 mousePosition;
        private Collider lastHit;
        private const int raycastRange = 100;
        [SerializeField]
        private LineRenderer lineOrigin;
        [SerializeField]
        private LayerMask defaultRaycastMask;
        [SerializeField]
        private float _followerStoppingDistance = 1.0f;

        private void OnEnable()
        {
            PlayerInputHandler.OnSecondaryAction += OnSecondaryAction;
            PlayerInputHandler.OnPointerPosition += OnPointerPosition;
        }

        private void OnDisable()
        {
            PlayerInputHandler.OnSecondaryAction -= OnSecondaryAction;
            PlayerInputHandler.OnPointerPosition -= OnPointerPosition;
        }

        private void OnSecondaryAction(InputAction.CallbackContext context)
        {
            if (InputHelper.IsRaycastHittingUIObject(mousePosition))
                return;
            if (context.performed)
                HandleWorldRightClick();
        }

        private void HandleWorldRightClick()
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, raycastRange, defaultRaycastMask))
            {
                switch (GameManager.Instance.GetCurrentState())
                {
                    case GameState.Combat:
                        {
                            HandleCombatMouseClick(hit);
                            break;
                        }
                    case GameState.Exploration:
                        {
                            List<ISelectable> selectables = Selection.Instance.GetAllSelected();
                            PlayerControlledCharacter currentLead;
                            if (selectables.Count > 0)
                            {
                                currentLead = selectables[0].GetCreature() as PlayerControlledCharacter;
                            }
                            else
                            {
                                break;
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
        }

        private void HandleCombatMouseClick(RaycastHit hit)
        {
            ActionBarUI.SelectedBarAction selectedBarAction = ActionBarUI.instance.GetSelectedBarAction();
            switch (selectedBarAction.actionType)
            {
                case BarAction.MOVE:
                    if (!TurnCombatManager.Instance.IsActiveActorPlayerControlled() && !DevMoveActorToPosition.InputOverride)
                        return;
                    else if (hit.collider.GetComponent<GridElement>())
                    {
                        if (hit.collider.gameObject.GetComponent<GridElement>().Visited != GridElement.Status.Occupied)
                        {
                            TurnCombatManager.Instance.CurrentActiveActor.Move(hit.collider.gameObject.GetComponent<GridElement>());
                        }
                    }
                    break;
                case BarAction.ATTACK:
                    if (!TurnCombatManager.Instance.IsActiveActorPlayerControlled() || GameManager.Instance.GetMainCharacter().GetComponent<CombatBehaviour>().HasActed)
                        return;
                    NPC enemy = hit.collider.GetComponent<NPC>();
                    if (enemy)
                    {
                        if (enemy.Data.Faction.Equals(Faction.Hostile) &&
                            Vector3.Distance(GameManager.Instance.GetMainCharacter().transform.position, enemy.transform.position) <= GameManager.Instance.GetMainCharacter().GetComponent<StatsManager>().GetWeaponMaxRange())
                        {
                            TurnCombatManager.Instance.CurrentActiveActor.Attack(enemy);
                        }
                    }
                    break;
            }
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
                switch (GameManager.Instance.GetCurrentState())
                {
                    case GameState.Exploration:
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
        }

        private void HandleCombatMousePosition(RaycastHit hit)
        {
            ActionBarUI.SelectedBarAction selectedBarAction = ActionBarUI.instance.GetSelectedBarAction();
            switch (selectedBarAction.actionType)
            {
                case BarAction.MOVE:
                    if (!TurnCombatManager.Instance.IsActiveActorPlayerControlled() && !DevMoveActorToPosition.InputOverride)
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
                    break;
                case BarAction.ATTACK:
                    if (!TurnCombatManager.Instance.IsActiveActorPlayerControlled() || GameManager.Instance.GetMainCharacter().GetComponent<CombatBehaviour>().HasActed)
                        return;
                    if (lastHit != hit.collider)
                    {
                        lineOrigin.enabled = false;
                        lastHit = hit.collider;
                    }
                    NPC creature = lastHit.GetComponent<NPC>();
                    if (creature)
                    {
                        lineOrigin.positionCount = 2;
                        lineOrigin.SetPosition(0, lineOrigin.transform.position);
                        lineOrigin.SetPosition(1, creature.LineTarget.position);
                        Color color = GameManager.Instance.GetMainCharacter().GetComponent<StatsManager>()
                            .GetWeaponMaxRange() >= Vector3.Distance(GameManager.Instance.GetMainCharacter().CurrentGridPosition.transform.position, creature.CurrentGridPosition.transform.position)
                            ? Color.green
                            : Color.red;
                        lineOrigin.startColor = color;
                        lineOrigin.endColor = color;
                        lineOrigin.enabled = true;
                    }
                    break;
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
