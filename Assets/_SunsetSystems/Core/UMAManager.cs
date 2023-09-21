using Sirenix.OdinInspector;
using SunsetSystems.Inventory.Data;
using System.Collections;
using System.Collections.Generic;
using UMA;
using UMA.CharacterSystem;
using UnityEngine;

namespace SunsetSystems.Core.UMA
{
    public class UMAManager : SerializedMonoBehaviour
    {
        [SerializeField, Required]
        private ScriptableUMAConfig umaConfig;
        [SerializeField, Required]
        private GameObject umaRoot;

        private DynamicCharacterAvatar umaAvatar;

        [Button]
        public void BuildUMA()
        {
            umaAvatar = umaRoot.GetComponent<DynamicCharacterAvatar>();
            if (umaAvatar == null)
                umaAvatar = umaRoot.AddComponent<DynamicCharacterAvatar>();
        }  

        public void OnItemEquipped(IEquipableItem item)
        {

        }

        public void OnItemUnequipped(IEquipableItem item)
        {

        }
    }
}
