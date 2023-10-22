using Sirenix.OdinInspector;
using SunsetSystems.Combat;
using SunsetSystems.Combat.Grid;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SunsetSystems.Input
{
    public class CombatInputHandler : SerializedMonoBehaviour, IGameplayInputHandler
    {
        [SerializeField]
        private LayerMask _raycastTargetMask;
        private const int raycastRange = 100;
        private Collider lastPointerHit;
        private Vector2 mousePosition;

        public void HandlePointerPosition(InputAction.CallbackContext context)
        {
            if (context.performed is false)
                return;
            mousePosition = context.ReadValue<Vector2>();
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            CombatManager.Instance.CurrentEncounter.GridManager.ClearHighlightedCell();
            if (Physics.Raycast(ray, out RaycastHit hit, raycastRange, _raycastTargetMask, QueryTriggerInteraction.Ignore))
            {
                if (lastPointerHit == null)
                {
                    lastPointerHit = hit.collider;
                }
                HandlePointerOverActionTarget(hit);
            }
        }

        public void HandlePrimaryAction(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void HandleSecondaryAction(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        private void HandlePointerOverActionTarget(RaycastHit hit)
        {
            if (CombatManager.Instance.IsActiveActorPlayerControlled() is false)
                return;
            HandleMoveActionPointerPosition();
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
                if (!CombatManager.Instance.IsActiveActorPlayerControlled())
                    return;
                IGridCell gridCell = hit.collider.gameObject.GetComponent<GridUnitObject>();
                if (gridCell != null)
                {
                    CombatManager.Instance.CurrentEncounter.GridManager.HighlightCell(gridCell);
                }
            }

            void HandleAttackActionPointerPosition()
            {
                if (!CombatManager.Instance.IsActiveActorPlayerControlled() || CombatManager.Instance.CurrentActiveActor.HasActed)
                    return;
                throw new NotImplementedException();
                //LineRenderer lineRenderer = CombatManager.CurrentActiveActor.LineRenderer;
                //if (lastHit != hit.collider)
                //{
                //    lineRenderer.enabled = false;
                //    lastHit = hit.collider;
                //}
                //ICreature creature = lastHit.GetComponent<ICreature>();
                //if (creature != null && creature.Faction is Faction.Hostile)
                //{
                //    lineRenderer.positionCount = 2;
                //    throw new NotImplementedException();
                //    //lineRenderer.SetPosition(0, lineRenderer.transform.position);
                //    //lineRenderer.SetPosition(1, creature.LineTarget.position);
                //    //Color color = IsInRange(creature)
                //    //    ? Color.green
                //    //    : Color.red;
                //    //lineRenderer.startColor = color;
                //    //lineRenderer.endColor = color;
                //    //lineRenderer.enabled = true;
                //}
            }
        }
    }
}
