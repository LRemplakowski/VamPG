using SunsetSystems.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Entities.Characters
{
    [CreateAssetMenu(fileName = "New Inventory Config", menuName = "Character/Inventory Config")]
    public class InventoryConfig : ScriptableObject
    {
        [field: SerializeField]
        public EquipmentData Equipment { get; private set; } = new();
        [field: SerializeField]
        public List<InventoryEntry> Inventory { get; private set; }
        [field: SerializeField]
        public float Money { get; private set; }
    }
}
