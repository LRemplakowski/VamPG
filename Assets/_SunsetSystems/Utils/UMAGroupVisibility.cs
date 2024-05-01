using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using SunsetSystems.Persistence;
using UMA.CharacterSystem;
using UnityEngine;

namespace SunsetSystems.Utils
{
    public class UMAGroupVisibility : SerializedMonoBehaviour, IPersistentComponent
    {
        [SerializeField]
        private List<DynamicCharacterAvatar> _childrenUMA = new();
        [SerializeField]
        private bool _umaVisible = true;
        private bool UmaVisible
        {
            get
            {
                return _umaVisible;
            }
            set
            {
                _umaVisible = value;
                RefreshUMAVisibility();
            }
        }

        public string ComponentID => "UMA_VISIBILITY_GROUP";

        private void OnValidate()
        {
            RefreshUMAs();
        }

        public void RefreshUMAs()
        {
            _childrenUMA = GetComponentsInChildren<DynamicCharacterAvatar>(true).ToList();
        }

        private void Start()
        {
            RefreshUMAVisibility();
        }

        public void SetUMAVisibility(bool visible)
        {
            UmaVisible = visible;
        }

        private void RefreshUMAVisibility()
        {
            foreach (var uma in _childrenUMA)
            {
                if (UmaVisible && uma.hide)
                    uma.Show();
                else
                    uma.Hide();
            }
        }

        public object GetComponentPersistenceData()
        {
            return new UMAGroupVisibilityPersistenceData(this);
        }

        public void InjectComponentPersistenceData(object data)
        {
            if (data is not UMAGroupVisibilityPersistenceData umaGroupData)
                return;
            _umaVisible = umaGroupData.UMAsVisible;
        }

        [SerializeField]
        public class UMAGroupVisibilityPersistenceData 
        {
            public bool UMAsVisible;

            public UMAGroupVisibilityPersistenceData(UMAGroupVisibility umaGroup)
            {
                UMAsVisible = umaGroup._umaVisible;
            }

            public UMAGroupVisibilityPersistenceData()
            {

            }
        }
    }
}
