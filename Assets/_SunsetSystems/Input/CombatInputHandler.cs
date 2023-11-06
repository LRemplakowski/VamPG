using Sirenix.OdinInspector;
using SunsetSystems.Combat;
using SunsetSystems.Combat.Grid;
using SunsetSystems.Entities.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SunsetSystems.Input
{
    public class CombatInputHandler : SerializedMonoBehaviour, IGameplayInputHandler
    {
        [Title("References")]
        [SerializeField, Required]
        private PlayerCombatActionManager selectedActionManager;
        [Title("Config")]
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
                lastPointerHit = hit.collider;
            }
        }

        public void HandlePrimaryAction(InputAction.CallbackContext context)
        {
            if (context.performed is false)
                return;
            if (CombatManager.Instance.IsActiveActorPlayerControlled() is false)
                return;
            if (lastPointerHit != null)
            {
                IGridCell gridCell = lastPointerHit.gameObject.GetComponent<IGridCell>();
                if (gridCell != null)
                {
                    ICombatant currentCombatant = CombatManager.Instance.CurrentActiveActor;
                    if (gridCell.IsFree && currentCombatant.HasMoved is false)
                    {
                        currentCombatant.MoveToGridPosition(gridCell.GridPosition);
                        CombatManager.Instance.CurrentEncounter.GridManager.HideCellsInMovementRange();
                    }
                }
            }
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
        }
    }
}
