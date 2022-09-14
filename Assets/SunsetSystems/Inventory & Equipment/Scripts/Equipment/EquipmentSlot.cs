using SunsetSystems.Inventory.Data;
using UnityEngine;

namespace SunsetSystems.Equipment
{
    [System.Serializable]
    public class EquipmentSlot : IEquipmentSlot
    {
        [SerializeField]
        private string _name;
        public string Name => _name;
        [SerializeField]
        private ItemCategory _acceptedCategory;
        public ItemCategory AcceptedCategory { get => _acceptedCategory; }
        [SerializeField]
        private EquipableItem _equippedItem;

        public EquipmentSlot(ItemCategory accepptedCategory)
        {
            this._acceptedCategory = accepptedCategory;
            this._name = accepptedCategory.ToString();
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
