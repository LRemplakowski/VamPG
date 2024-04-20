using System;
using Sirenix.OdinInspector;
using SunsetSystems.Core.Database;
using SunsetSystems.Inventory.Data;
using UnityEngine;

namespace SunsetSystems.Equipment
{
    [Serializable, HideReferenceObjectPicker]
    public class EquipmentSlot : IEquipmentSlot
    {
        [field: SerializeField, ReadOnly]
        public EquipmentSlotID ID { get; private set; }

        [ShowInInspector, ReadOnly]
        public string DefaultItemID
        {
            get
            {
                return ID switch
                {
                    EquipmentSlotID.PrimaryWeapon => "IT_WPN_BR_FISTS",
                    EquipmentSlotID.SecondaryWeapon => "IT_WPN_BR_FISTS",
                    EquipmentSlotID.Chest => "IT_CLTH_DEFAULT",
                    EquipmentSlotID.Boots => "IT_BTS_DEFAULT",
                    EquipmentSlotID.Hands => "IT_HDS_DEFAULT",
                    EquipmentSlotID.Trinket => "IT_TRKT_DEFAULT",
                    EquipmentSlotID.Invalid => throw new ArgumentException("Requested default item ID for Invalid slot!"),
                    _ => throw new NotImplementedException($"Equipment slot {ID} does not have default item ID!"),
                };
            }
        }

        [ShowInInspector, ReadOnly]
        public ItemCategory AcceptedCategory
        {
            get
            {
                return ID switch
                {
                    EquipmentSlotID.PrimaryWeapon => ItemCategory.WEAPON,
                    EquipmentSlotID.SecondaryWeapon => ItemCategory.WEAPON,
                    EquipmentSlotID.Chest => ItemCategory.CLOTHING,
                    EquipmentSlotID.Boots => ItemCategory.SHOES,
                    EquipmentSlotID.Hands => ItemCategory.GLOVES,
                    EquipmentSlotID.Trinket => ItemCategory.TRINKET,
                    EquipmentSlotID.Invalid => throw new ArgumentException($"Requested Accepted Item Category for Invalid slot!"),
                    _ => throw new NotImplementedException($"Equipment slot {ID} does not have Accepted Item Category!"),
                };
            }
        }

        public IEquipmentSlot Data => this;

        [SerializeField]
        private IEquipableItem _equippedItem;

        public EquipmentSlot(EquipmentSlotID id)
        {
            ID = id;
            _equippedItem = default;
        }

        public EquipmentSlot(IEquipmentSlot slot)
        {
            ID = slot.ID;
            _equippedItem = slot.GetEquippedItem();
        }

        public EquipmentSlot()
        {
            ID = EquipmentSlotID.Invalid;
            _equippedItem = default;
        }

        public IEquipableItem GetEquippedItem()
        { 
            return _equippedItem;
        }

        public bool TryEquipItem(IEquipableItem item, out IEquipableItem unequipped)
        {
            unequipped = default;
            if (item == null)
                return false;
            if (AcceptedCategory.Equals(item.ItemCategory) is false)
                return false;
            if (_equippedItem != null)
            {
                TryUnequipItem(out unequipped);
                _equippedItem = item;
                return true;
            }
            else
            {
                _equippedItem = item;
                return true;
            }
        }

        public bool TryUnequipItem(out IEquipableItem item)
        {
            item = _equippedItem;
            if (ItemDatabase.Instance.TryGetEntryByReadableID(DefaultItemID, out IBaseItem defaultItem) && defaultItem is IEquipableItem equipableItem)
                _equippedItem = equipableItem;
            return item != null || item.ReadableID != DefaultItemID;
        }
    }
}
