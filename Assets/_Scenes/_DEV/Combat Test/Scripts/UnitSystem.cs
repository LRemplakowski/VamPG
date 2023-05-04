using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System;
using SunsetSystems.Input.CameraControl;

namespace SunsetSystems
{
    public class UnitSystem : MonoBehaviour
    {

        public static UnitSystem Instance { get; private set; }
        
        public event EventHandler OnSelectedUnitChanged;
        public event EventHandler OnSelectedActionChanged;
        public event EventHandler<bool> OnBusyChanged;
        public event EventHandler OnActionStarted;

        [SerializeField] private Unit selectedUnit;
        [SerializeField] private LayerMask unitLayerMask;

        private Vector2 mousePos;

        private BaseAction selectedAction;
        private bool isBusy;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("There is more than one Unit System! " + transform + " - " + Instance);
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void OnEnable()
        {
            PlayerInputHandler.OnPrimaryAction += OnPrimaryAction;
            PlayerInputHandler.OnPointerPosition += OnMousePosition;
        }

        private void OnDisable()
        {
            PlayerInputHandler.OnPointerPosition -= OnMousePosition;
            PlayerInputHandler.OnPrimaryAction -= OnPrimaryAction;
        }

        private void Start()
        {
            SetSelectedUnit(selectedUnit);
        }

        public void HandleSelectedAction()
        {
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(CameraControlScript.GetPosition());

            if (!selectedAction.IsValidActionGridPosition(mouseGridPosition)){
                return;
            }
            if (!selectedUnit.TrySpendActionPointsToTakeAction(selectedAction)){
                return;
            }
                    
            SetBusy();
            selectedAction.TakeAction(mouseGridPosition, ClearBusy);

            OnActionStarted?.Invoke(this, EventArgs.Empty);   
        }   

        private void SetBusy()
        {
            isBusy = true;

            OnBusyChanged?.Invoke(this, isBusy);
        }

        private void ClearBusy()
        {
            isBusy = false;

            OnBusyChanged?.Invoke(this, isBusy);
        }

        public void OnPrimaryAction(InputAction.CallbackContext context)
        {
            Debug.Log("primary action");
            if (context.performed is false)
                return;

            if (ShouldSkipPrimaryAction())
                return;

            if (TryHandleUnitSelection())
                return;
            else
                HandleSelectedAction();
        }

        private bool ShouldSkipPrimaryAction()
        {
            return isBusy || !TurnSystem.Instance.IsPlayerTurn() || EventSystem.current.IsPointerOverGameObject();
        }

        public void OnMousePosition(InputAction.CallbackContext context)
        {
            if (context.performed)
                mousePos = context.ReadValue<Vector2>();
            Debug.Log($"{mousePos}");
        }

        public bool TryHandleUnitSelection()
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitLayerMask))
            {
                if (raycastHit.transform.TryGetComponent<Unit>(out Unit unit))
                {
                    if (unit == selectedUnit)
                    {
                        // Unit is already selected.
                        return false;
                    }
                            
                    if (unit.IsEnemy()){
                        // Clicked on an enemy.
                        return false;
                    }

                    SetSelectedUnit(unit);
                    return true;
                }
            }
            return false;
        }

        private void SetSelectedUnit(Unit unit)
        {
            selectedUnit = unit;

            SetSelectedAction(unit.GetMoveAction());

            OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
        }

        
        public void SetSelectedAction(BaseAction baseAction)
        {
            selectedAction = baseAction;

            OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
        }
        
        public Unit GetSelectedUnit()
        {
            return selectedUnit;
        }

        
        public BaseAction GetSelectedAction()
        {
            return selectedAction;
        }

    }
}
