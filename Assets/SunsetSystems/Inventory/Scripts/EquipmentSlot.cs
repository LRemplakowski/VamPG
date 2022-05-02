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
        private ItemType[] _acceptedCategories = new ItemType[0];
        private ItemType[] _uniqueCategories;
        public List<ItemType> ItemCategories => _uniqueCategories.ToList();

        private void OnValidate()
        {
            _uniqueCategories = _acceptedCategories.Distinct().ToArray();
        }
    }
}
