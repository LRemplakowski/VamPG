using System;
using Sirenix.OdinInspector;
using SunsetSystems.Abilities;
using SunsetSystems.Combat;
using SunsetSystems.Combat.Grid;
using SunsetSystems.Game;
using SunsetSystems.Input.CameraControl;
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
        private LayerMask _targetableLayerMask;

        private const int raycastRange = 100;
        private Vector2 _pointerPosition;

        private bool _pointerOverGameObject;
        private bool _showTargetingLine;

        private Collider _lastHitCollider;
        private ITargetable _targetableComponent;

        public delegate void TargetingContextDelegate(ITargetable target);
        public static event TargetingContextDelegate OnTargetingDataUpdate;

        private ITargetingContext _targetingContext;

        private void Awake()
        {
            SetTargetingContext(new AbilityTargetingContext(this));
        }

        private void Start()
        {
            SetShowTargetingLine(false);
            UpdateTargetingLineActive();
            _selectedActionManager.Initialize(GetTargetingContext());
        }

        private void Update()
        {
            if (CombatManager.Instance.IsActiveActorPlayerControlled() is false || GameManager.Instance.IsCurrentState(GameState.Combat) is false)
                SetShowTargetingLine(false);
            UpdatePointerOverGameObject();
            UpdateTargetingLineActive();
        }

        private void UpdatePointerOverGameObject()
        {
            _pointerOverGameObject = EventSystem.current.IsPointerOverGameObject();
        }

        private void UpdateTargetingLineActive()
        {
            GetTargetingLineRenderer().enabled = GetShowTargetingLine();
        }

        public void HandlePointerPosition(InputAction.CallbackContext context)
        {
            if (context.performed is false)
                return;
            if (CombatManager.Instance.IsActiveActorPlayerControlled() is false)
                return;
            SetPointerPosition(context.ReadValue<Vector2>());
            CombatManager.Instance.CurrentEncounter.GridManager.ClearHighlightedCell();
            if (_pointerOverGameObject)
            {
                SetLastHitCollider(null);
                SetCurrentTarget(null);
                return;
            }
            Ray ray = Camera.main.ScreenPointToRay(GetPointerPosition());
            if (Physics.Raycast(ray, out var hit, raycastRange, _targetableLayerMask) && GetLastHitCollider() != hit.collider)
            {
                SetLastHitCollider(hit.collider);
                _selectedActionManager.SelectedAbility.GetTargetingStrategy().ExecutePointerPosition(GetTargetingContext());
            }
            else
            {
                SetLastHitCollider(null);
                SetCurrentTarget(null);
                SetShowTargetingLine(false);
            }
        }

        public void HandlePrimaryAction(InputAction.CallbackContext context)
        {
            if (context.started is false)
                return;
            if (CombatManager.Instance.IsActiveActorPlayerControlled() is false)
                return;
        }

        public void HandleSecondaryAction(InputAction.CallbackContext context)
        {
            if (context.started is false)
                return;
            if (CombatManager.Instance.IsActiveActorPlayerControlled() is false)
                return;
        }

        public void HandleCameraMoveAction(InputAction.CallbackContext context)
        {
            CameraControlScript.Instance.OnMove(context);
        }

        private ITargetingContext GetTargetingContext() => _targetingContext;
        private void SetTargetingContext(ITargetingContext context) => _targetingContext = context;

        private Vector2 GetPointerPosition() => _pointerPosition;
        private void SetPointerPosition(Vector2 position) => _pointerPosition = position;

        private ITargetable GetCurrentTarget() => _targetableComponent;
        private void SetCurrentTarget(ITargetable target)
        {
            _targetableComponent = target;
            OnTargetingDataUpdate?.Invoke(target);
        }

        private bool GetShowTargetingLine() => _showTargetingLine;
        private void SetShowTargetingLine(bool show) => _showTargetingLine = show;

        private LineRenderer GetTargetingLineRenderer() => _targetingLineRenderer;

        private Collider GetLastHitCollider() => _lastHitCollider;
        private void SetLastHitCollider(Collider collider) => _lastHitCollider = collider;

        private class AbilityTargetingContext : ITargetingContext
        {
            private readonly CombatInputHandler _inputHandler;

            public AbilityTargetingContext(CombatInputHandler inputHandler)
            {
                _inputHandler = inputHandler;
            }

            public IAbilityContext GetAbilityContext()
            {
                return GetCurrentCombatant().GetContext().AbilityUser.GetCurrentAbilityContext();
            }

            public ICombatant GetCurrentCombatant()
            {
                return CombatManager.Instance.CurrentActiveActor;
            }

            public GridManager GetCurrentGrid()
            {
                return CombatManager.Instance.CurrentEncounter.GridManager;
            }

            public Collider GetLastRaycastCollider()
            {
                return _inputHandler.GetLastHitCollider();
            }

            public IAbilityConfig GetSelectedAbility()
            {
                return _inputHandler._selectedActionManager.SelectedAbility;
            }

            public LineRenderer GetTargetingLineRenderer()
            {
                return _inputHandler.GetTargetingLineRenderer();
            }

            public Action<ITargetable> UpdateTargetDelegate()
            {
                return _inputHandler.SetCurrentTarget;
            }

            public Action<bool> UpdateTargetingLineVisibilityDelegate()
            {
                return _inputHandler.SetShowTargetingLine;
            }
        }
    }
}
