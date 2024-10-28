using Sirenix.OdinInspector;
using Sirenix.Serialization;
using SunsetSystems.Equipment;
using SunsetSystems.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Entities.Characters
{
    [CreateAssetMenu(fileName = "New Inventory Config", menuName = "Character/Inventory Config")]
    public class InventoryConfig : SerializedScriptableObject
    {
        [OdinSerialize]
        public List<InventoryEntry> Inventory { get; private set; }
        [field: SerializeField]
        public float Money { get; private set; }
    }
}
