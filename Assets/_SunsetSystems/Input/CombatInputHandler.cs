using Sirenix.OdinInspector;
using SunsetSystems.Combat;
using SunsetSystems.Combat.Grid;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Game;
using SunsetSystems.Input.CameraControl;
using SunsetSystems.Spellbook;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace SunsetSystems.Input
{
    public class CombatInputHandler : SerializedMonoBehaviour, IGameplayInputHandler
    {
        [Title("References")]
        [SerializeField, Required]
        private PlayerCombatActionManager _selectedActionManager;
        [SerializeField]
        private LineRenderer _targetingLineRenderer;
        [Title("Config")]
        [SerializeField]
        private LayerMask gridLayerMask;
        [SerializeField]
        private LayerMask targetableLayerMask;

        private const int raycastRange = 100;
        private Vector2 mousePosition;

        private bool _pointerOverGameObject;
        private bool _showTargetingLine;

        private Collider gridHit;
        private Collider targetableHit;

        private void Start()
        {
            _targetingLineRenderer.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (CombatManager.Instance.IsActiveActorPlayerControlled() is false || GameManager.Instance.IsCurrentState(GameState.Combat) is false)
                _showTargetingLine = false;
            _pointerOverGameObject = EventSystem.current.IsPointerOverGameObject();
            _targetingLineRenderer.gameObject.SetActive(_showTargetingLine);
        }

        public void HandlePointerPosition(InputAction.CallbackContext context)
        {
            if (context.performed is false)
                return;
            mousePosition = context.ReadValue<Vector2>();
            if (CombatManager.Instance.IsActiveActorPlayerControlled() is false)
                return;
            CombatManager.Instance.CurrentEncounter.GridManager.ClearHighlightedCell();
            if (_pointerOverGameObject)
            {
                _selectedActionManager.SetLastGridHit(null);
                gridHit = null;
                _selectedActionManager.SetLastTargetableHit(null);
                targetableHit = null;
                return;
            }
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, raycastRange, gridLayerMask))
            {
                HandleGridCellPointerPosition();
                gridHit = hit.collider;
                _selectedActionManager.SetLastGridHit(gridHit);
            }
            if (Physics.Raycast(ray, out hit, raycastRange, targetableLayerMask))
            {
                if (targetableHit != hit.collider)
                    HandleTargetablePointerPosition();
                targetableHit = hit.collider;
                _selectedActionManager.SetLastTargetableHit(targetableHit);
            }
            else
            {
                targetableHit = null;
                _selectedActionManager.SetLastTargetableHit(null);
                _showTargetingLine = false;
            }

            void HandleGridCellPointerPosition()
            {
                if (hit.collider.gameObject.TryGetComponent<IGridCell>(out var gridCell))
                {
                    CombatManager.Instance.CurrentEncounter.GridManager.HighlightCell(gridCell);
                }
            }

            void HandleTargetablePointerPosition()
            {
                if (hit.collider.gameObject.TryGetComponent(out ICreature creature))
                {
                    _showTargetingLine = (_selectedActionManager.SelectedActionData.ActionType & CombatActionType.RangedAtk) > 0;
                    ICombatant current = CombatManager.Instance.CurrentActiveActor;
                    ICombatant target = creature.References.CombatBehaviour;
                    if (target.IsAlive && current.IsTargetInRange(target))
                    {
                        _targetingLineRenderer.SetPosition(0, current.AimingOrigin);
                        _targetingLineRenderer.SetPosition(1, target.AimingOrigin);
                        current.References.NavigationManager.FaceDirectionAfterMovementFinished(target.References.Transform.position);
                    }
                    else
                    {
                        _showTargetingLine = false;
                    }
                }
            }
        }

        public void HandlePrimaryAction(InputAction.CallbackContext context)
        {
            if (context.started is false)
                return;
            if (CombatManager.Instance.IsActiveActorPlayerControlled() is false)
                return;
            if (_selectedActionManager.SelectedActionData.ExecuteImmediate is false)
                _selectedActionManager.ExecuteAction(_selectedActionManager.SelectedActionData);
        }

        public void HandleSecondaryAction(InputAction.CallbackContext context)
        {
            if (context.performed is false)
                return;
            if (CombatManager.Instance.IsActiveActorPlayerControlled() is false)
                return;
            switch (_selectedActionManager.SelectedActionData.ActionType)
            {
                case CombatActionType.Move:
                    break;
                case CombatActionType.RangedAtk:
                    break;
                case CombatActionType.MeleeAtk:
                    break;
                case CombatActionType.Feed:
                    break;
                case CombatActionType.Reload:
                    break;
            }
        }

        public void HandleCameraMoveAction(InputAction.CallbackContext context)
        {
            CameraControlScript.Instance.OnMove(context);
        }
    }
}
