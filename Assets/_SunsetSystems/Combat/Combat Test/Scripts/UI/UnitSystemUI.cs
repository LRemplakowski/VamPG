using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SunsetSystems.Combat
{
    public class UnitSystemUI : MonoBehaviour
    {

        [SerializeField] private Transform actionButtonPrefab;
        [SerializeField] private Transform actionButtonContainerTransform;
        [SerializeField] private TextMeshProUGUI actionPointsText;

        private List<UnitSystemButton> unitSystemButtonList;

        private void Awake(){
            unitSystemButtonList = new List<UnitSystemButton>();
        }
        
        
        private void Start()
        {
            UnitSystem.Instance.OnSelectedUnitChanged += UnitSystem_OnSelectedUnitChanged;
            UnitSystem.Instance.OnSelectedActionChanged += UnitSystem_OnSelectedActionChanged;
            UnitSystem.Instance.OnActionStarted += UnitSystem_OnActionStarted;
            TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;

            UpdateActionPoints();
            CreateUnitActionButtons();
            UpdateSelectedVisual();
        }


        private void CreateUnitActionButtons()
        {
            foreach (Transform buttonTransform in actionButtonContainerTransform)
            {
                Destroy(buttonTransform.gameObject);
            }

            unitSystemButtonList.Clear();

            Unit selectedUnit = UnitSystem.Instance.GetSelectedUnit();

            foreach (BaseAction baseAction in selectedUnit.GetBaseActionArray())
            {
                Transform actionButtonTransform = Instantiate(actionButtonPrefab, actionButtonContainerTransform);
                UnitSystemButton unitSystemButton = actionButtonTransform.GetComponent<UnitSystemButton>();
                unitSystemButton.SetBaseAction(baseAction);

                unitSystemButtonList.Add(unitSystemButton);
            }
        }

        private void UnitSystem_OnSelectedUnitChanged(object sender, EventArgs e)
        {
            CreateUnitActionButtons();
            UpdateSelectedVisual();
            UpdateActionPoints();
        }

        private void UnitSystem_OnSelectedActionChanged(object sender, EventArgs e)
        {
            UpdateSelectedVisual();
        }

        private void UnitSystem_OnActionStarted(object sender, EventArgs e){
            UpdateActionPoints();
        }

        private void UpdateSelectedVisual(){
            foreach (UnitSystemButton unitSystemButton in unitSystemButtonList){
                unitSystemButton.UpdateSelectedVisual();
            }
        }

        private void UpdateActionPoints(){
            Unit selectedUnit = UnitSystem.Instance.GetSelectedUnit();

            actionPointsText.text = "Action Points: " + selectedUnit.GetActionPoints();
        }

        private void TurnSystem_OnTurnChanged(object sender, EventArgs e){
            UpdateActionPoints();
        }

    }
}
