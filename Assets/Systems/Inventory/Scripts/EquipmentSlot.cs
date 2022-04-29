using SunsetSystems.Inventory.Data;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SunsetSystems.Inventory
{
    [CreateAssetMenu(fileName = "New Equipment Slot", menuName = "Inventory/Equipment Slot")]
    public class EquipmentSlot : ScriptableObject
    {
        [SerializeField]
        private Sprite _defaultIcon;
        [SerializeField]
        private ItemCategory[] _acceptedCategories = new ItemCategory[0];
        private ItemCategory[] _uniqueCategories;
        public List<ItemCategory> ItemCategories => _uniqueCategories.ToList();

        private void OnValidate()
        {
            _uniqueCategories = _acceptedCategories.Distinct().ToArray();
        }
    }
}
