using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace SunsetSystems.MainMenu.UI
{
    [RequireComponent(typeof(TMP_Dropdown))]
    public class DotsPatternSelection : MonoBehaviour
    {
        [SerializeField]
        private UnassignedDots unassignedDots;
        [SerializeField, Tooltip("Used only when Unassigned Dots is null.")]
        private string unassginedDotsTag;
        [SerializeField]
        private TMP_Dropdown dropdown;
        [SerializeField]
        private GroupPattern[] groupPatterns;

        private void Start()
        {
            if (unassignedDots == null)
                unassignedDots = GameObject.FindGameObjectWithTag(unassginedDotsTag).GetComponent<UnassignedDots>();
            unassignedDots.SetGroupsData(groupPatterns[dropdown.value].DotGroupDatas);
        }

        private void OnEnable()
        {
            if (dropdown == null)
                dropdown = GetComponent<TMP_Dropdown>();
            dropdown.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnDisable()
        {
            dropdown.onValueChanged.RemoveListener(OnValueChanged);
        }

        private void OnValueChanged(int value)
        {
            unassignedDots.SetGroupsData(groupPatterns[value].DotGroupDatas);
        }

        [System.Serializable]
        private class GroupPattern
        {
            [SerializeField]
            private DotGroupData[] _groupDatas;
            public DotGroupData[] DotGroupDatas { get => _groupDatas; }
        }

        [ContextMenu("Init current value")]
        private void InitCurrentValue()
        {
            if (dropdown == null)
                dropdown = GetComponent<TMP_Dropdown>();
            if (unassignedDots == null)
                unassignedDots = GameObject.FindGameObjectWithTag(unassginedDotsTag).GetComponent<UnassignedDots>();
            unassignedDots.SetGroupsData(groupPatterns[dropdown.value].DotGroupDatas);
        }
    }
}
