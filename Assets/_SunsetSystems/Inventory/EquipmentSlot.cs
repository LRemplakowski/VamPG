using Sirenix.OdinInspector;
using SunsetSystems.Inventory.Data;
using SunsetSystems.UI.Utils;
using UnityEngine;

namespace SunsetSystems.Inventory
{
    [System.Serializable]
    public class EquipmentSlot : IEquipmentSlot, IGameDataProvider<EquipmentSlot>
    {
        [field: SerializeField, ReadOnly]
        public string Name { get; private set; }
        [field: SerializeField, ReadOnly]
        public string ID { get; private set; }
        [SerializeField]
        private ItemCategory _acceptedCategory;
        public ItemCategory AcceptedCategory { get => _acceptedCategory; }

        public EquipmentSlot Data => this;

        [SerializeField]
        private EquipableItem _equippedItem;

        public EquipmentSlot(ItemCategory acceptedCategory, string name, string id)
        {
            this._acceptedCategory = acceptedCategory;
            this.Name = name;
            this.ID = id;
            _equippedItem = null;
        }

        public EquipmentSlot()
        {

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
