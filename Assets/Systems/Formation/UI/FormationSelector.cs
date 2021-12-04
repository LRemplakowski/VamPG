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
        FormationController formationController;

        private void Start()
        {
            formationController = FindObjectOfType<FormationController>();
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
                TMP_Dropdown.OptionData data = new TMP_Dropdown.OptionData();
                data.image = formation.GetSprite();
                options.Add(data);
            }
            return options;
        }

        public void OnSelectionChanged(int index)
        {
            formationController.FormationData = formations[index].GetData();
        }
    }
}

