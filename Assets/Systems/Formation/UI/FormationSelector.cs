using System.Collections;
using System.Collections.Generic;
using SunsetSystems.Formation.UI;
using UnityEngine;
using TMPro;
using SunsetSystems.Management;

namespace SunsetSystems.Formation.UI
{
    public class FormationSelector : ExposableMonobehaviour
    {
        [SerializeField]
        private PredefinedFormation[] formations = new PredefinedFormation[0];

        private void Start()
        {
            TMP_Dropdown dropdown = GetComponent<TMP_Dropdown>();
            dropdown.ClearOptions();
            dropdown.AddOptions(GetOptions());
            OnSelectionChanged(dropdown.value);
        }

        private List<TMP_Dropdown.OptionData> GetOptions()
        {
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
            foreach (PredefinedFormation formation in formations)
            {
                TMP_Dropdown.OptionData data = new TMP_Dropdown.OptionData
                {
                    image = formation.GetSprite()
                };
                options.Add(data);
            }
            return options;
        }

        public void OnSelectionChanged(int index)
        {
            FormationController.FormationData = formations[index].GetData();
        }
    }
}

