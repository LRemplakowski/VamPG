using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SunsetSystems
{
    public class UnitSystemButton : MonoBehaviour
    {

        [SerializeField] private TextMeshProUGUI textMeshPro;
        [SerializeField] private Button button;
        [SerializeField] private GameObject selectedGameObject;

        private BaseAction baseAction;

        public void SetBaseAction(BaseAction baseAction)
        {
            this.baseAction = baseAction;
            textMeshPro.text = baseAction.GetActionName().ToUpper();
            button.onClick.AddListener(() => {
                UnitSystem.Instance.SetSelectedAction(baseAction);
            });
        }

        public void UpdateSelectedVisual(){
            BaseAction selectedBaseAction = UnitSystem.Instance.GetSelectedAction();
            selectedGameObject.SetActive(selectedBaseAction == baseAction);
        }
    }
}
