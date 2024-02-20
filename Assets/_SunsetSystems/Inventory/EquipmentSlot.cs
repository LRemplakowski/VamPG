using Sirenix.OdinInspector;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Inventory.Data;
using SunsetSystems.UI.Utils;
using UnityEngine;

namespace SunsetSystems.Equipment
{
    [System.Serializable]
    public class EquipmentSlot : IEquipmentSlot
    {
        [field: SerializeField, ReadOnly]
        public EquipmentSlotID ID { get; private set; }
        [SerializeField, ReadOnly]
        private ItemCategory _acceptedCategory;
        public ItemCategory AcceptedCategory { get => _acceptedCategory; }

        public IEquipmentSlot Data => this;

        [SerializeField]
        private IEquipableItem _equippedItem;

        public EquipmentSlot(ItemCategory acceptedCategory, EquipmentSlotID id)
        {
            this._acceptedCategory = acceptedCategory;
            this.ID = id;
            _equippedItem = null;
        }

        public EquipmentSlot(IEquipmentSlot slot)
        {
            this._acceptedCategory = slot.AcceptedCategory;
            this.ID = slot.ID;
            _equippedItem = slot.GetEquippedItem();
        }

        public EquipmentSlot()
        {

        }

        public IEquipableItem GetEquippedItem()
        {
            return _equippedItem;
        }

        public bool TryEquipItem(IEquipableItem item)
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

        public bool TryUnequipItem(IEquipableItem item)
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
