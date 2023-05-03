using CleverCrow.Fluid.UniqueIds;
using SunsetSystems.Constants;
using SunsetSystems.Game;
using SunsetSystems.Loading;
using SunsetSystems.Utils;
using System.EnterpriseServices;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using SunsetSystems.Input.CameraControl;

namespace SunsetSystems
{
    public class UnitSystem : ExposableMonobehaviour
    {

        public static UnitSystem Instance { get; private set; }
        
        public event EventHandler OnSelectedUnitChanged;
        public event EventHandler OnSelectedActionChanged;
        public event EventHandler<bool> OnBusyChanged;
        public event EventHandler OnActionStarted;

        [SerializeField] private Unit selectedUnit;
        [SerializeField] private LayerMask unitLayerMask;

        private Vector2 mousePos;
        private Vector3 mousePosVector;

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

        private void Start()
        {
            SetSelectedUnit(selectedUnit);
        }

        private void Update()
        {
            if (isBusy)
            {
                return;
            }

            if (!TurnSystem.Instance.IsPlayerTurn()){
                return;
            }

            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            if (TryHandleUnitSelection())
            {
                return;
            }

            HandleSelectedAction();
        }

        public void HandleSelectedAction()
        {
            if(Mouse.current.leftButton.wasPressedThisFrame){
                
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

        public bool TryHandleUnitSelection()
        {
            if(Mouse.current.leftButton.wasPressedThisFrame){
                mousePos = new Vector2(Mouse.current.position.x.ReadValue(),
                                            Mouse.current.position.y.ReadValue());
                mousePosVector = new Vector3(mousePos.x, 0, mousePos.y);
                Ray ray = Camera.main.ScreenPointToRay(mousePosVector);
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
