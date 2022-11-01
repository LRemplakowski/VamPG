using SunsetSystems.Inventory.Data;
using UnityEngine;

namespace SunsetSystems.Equipment
{
    [System.Serializable]
    public class EquipmentSlot : IEquipmentSlot
    {
        [SerializeField, HideInInspector]
        private string _name;
        [SerializeField]
        private ItemCategory _acceptedCategory;
        public ItemCategory AcceptedCategory { get => _acceptedCategory; }
        [SerializeField]
        private EquipableItem _equippedItem;

        public EquipmentSlot(ItemCategory _acceptedCategory)
        {
            this._acceptedCategory = _acceptedCategory;
            _name = _acceptedCategory.ToString();
        }

        public EquipableItem GetEquippedItem()
        {
            return _equippedItem;
        }

        public bool TryEquipItem(EquipableItem item)
        {
            if (!_acceptedCategory.Equals(item.ItemCategory))
                return false;
            if (_equippedItem != null)
            {
                if (TryUnequipItem(_equippedItem))
                {
                    _equippedItem = item;
                    return true;
                }
                return false;
            }
            else
            {
                _equippedItem = item;
                return true;
            }
        }

        public bool TryUnequipItem(EquipableItem item)
        {
            if (_equippedItem == null)
                return false;
            if (_equippedItem.Equals(item))
            {
                _equippedItem = null;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
