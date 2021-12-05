using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.MainMenu.UI
{
    public class UnassignedDots : MonoBehaviour
    {
        [SerializeField]
        private DotGroup dotGroupPrefab;
        [SerializeField]
        private DotGroupData[] groupsData;

        private bool isDirty = false;

        internal delegate void OnDotGroupDataChanged();
        internal event OnDotGroupDataChanged onDotGroupDataChanged;

        private void Awake()
        {
            RebuildGroups();
        }

        private void OnEnable()
        {
            onDotGroupDataChanged += EnableAllDotGroups;
        }

        private void OnDisable()
        {
            onDotGroupDataChanged -= EnableAllDotGroups;
        }

        private void EnableAllDotGroups()
        {
            foreach (DotGroupData data in groupsData)
                data.isAvailable = true;
        }

        private void Update()
        {
            if (isDirty)
                RebuildGroups();
        }

        public bool HasEnabledDotGroupWithCount(int dotCount)
        {
            return Array.Exists(groupsData, d => d.isAvailable && d.DotCount == dotCount);
        }

        public bool EnableDotGroup(int dotCount)
        {
            DotGroupData data = Array.Find(groupsData, d => !d.isAvailable && d.DotCount == dotCount);
            if (data != null)
            {
                data.isAvailable = true;
                isDirty = true;
                return true;
            }
            return false;
        }

        public bool DisableDotGroup(int dotCount)
        {
            DotGroupData data = Array.Find(groupsData, d => d.isAvailable && d.DotCount == dotCount);
            if (data != null)
            {
                data.isAvailable = false;
                isDirty = true;
                return true;
            }
            return false;
        }

        [ContextMenu("Rebuild groups")]
        private void ContextRebuildGroups()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
            InitGroups();
        }

        private void RebuildGroups()
        {
            ClearGroups();
            InitGroups();
            isDirty = false;
        }

        private void InitGroups()
        {
            foreach (DotGroupData data in groupsData)
            {
                if (data.isAvailable)
                {
                    DotGroup group = Instantiate(dotGroupPrefab, transform);
                    group.FullDots = data.DotCount;
                    group.RebuildDotGroup();
                }
            }
        }

        private void ClearGroups()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        internal void SetGroupsData(DotGroupData[] groupsData)
        {
            this.groupsData = groupsData.Clone() as DotGroupData[];
            onDotGroupDataChanged?.Invoke();
            isDirty = true;
        }
    }

    [Serializable]
    internal class DotGroupData
    {
        [SerializeField]
        private int _dotCount;
        public int DotCount { get => _dotCount; }
        [SerializeField]
        public bool isAvailable = true;

        public DotGroupData()
        {
            _dotCount = 0;
            isAvailable = true;
        }
    }
}
