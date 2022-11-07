using CleverCrow.Fluid.UniqueIds;
using SunsetSystems.Entities.Characters;
using NaughtyAttributes;
using SunsetSystems.Equipment;
using SunsetSystems.Inventory.Data;
using SunsetSystems.Loading;
using SunsetSystems.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Inventory
{
    [RequireComponent(typeof(ItemStorage)), RequireComponent(typeof(UniqueId))]
    public class InventoryManager : Singleton<InventoryManager>, ISaveRuntimeData
    {
        [SerializeField]
        private ItemStorage _playerInventory;
        [SerializeField]
        private int _money;
        public static ItemStorage PlayerInventory => Instance._playerInventory;
        [SerializeField, ES3Serializable]
        private SerializableStringEquipmentDataDictionary _coterieEquipmentData = new();
        private UniqueId _unique;

        public static event Action<Creature, EquipableItem> ItemEquipped, ItemUnequipped;

        protected override void Awake()
        {
            base.Awake();
            if (!_playerInventory)
                _playerInventory = GetComponent<ItemStorage>();
            if (!_playerInventory)
                _playerInventory = gameObject.AddComponent(typeof(ItemStorage)) as ItemStorage;
            _unique ??= GetComponent<UniqueId>();
        }

        public static bool TryAddCoterieMemberEquipment(Creature character)
        {
            return Instance._coterieEquipmentData.TryAdd(character.Data.FullName, new());
        }

        public static bool TryGetEquipmentData(Creature character, out EquipmentData data)
        {
            return Instance._coterieEquipmentData.TryGetValue(character.Data.FullName, out data);
        }

        public static bool TryEquipItemInSlot(Creature character, int slotID, EquipableItem item)
        {
            if (Instance._coterieEquipmentData.TryGetValue(character.Data.FullName, out EquipmentData equipmentData) == false)
                return false;
            try
            {
                EquipmentSlot slot = equipmentData.equipmentSlots[slotID];
                if (slot.GetEquippedItem() != null)
                {
                    if (TryUnequipItemFromSlot(character, slotID) == false)
                    {
                        Debug.LogError("Could not unequip item " + slot.GetEquippedItem().ItemName + " from slot " + slotID + " for creature " + character.gameObject.name + "!");
                        return false;
                    }
                }
                bool success = slot.TryEquipItem(item);
                if (success)
                    ItemEquipped?.Invoke(character, item);
                return success;
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
        }

        public static bool TryUnequipItemFromSlot(Creature character, int slotID)
        {
            if (Instance._coterieEquipmentData.TryGetValue(character.Data.FullName, out EquipmentData equipmentData) == false)
                return false;
            try
            {
                EquipmentSlot slot = equipmentData.equipmentSlots[slotID];
                EquipableItem item = slot.GetEquippedItem();
                bool success = slot.TryUnequipItem(item);
                if (success)
                    ItemUnequipped?.Invoke(character, item);
                return success;
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
        }

        public static void TransferItem(ItemStorage from, ItemStorage to, InventoryEntry item)
        {
            if (from.TryRemoveItem(item))
            {
                to.AddItem(item);
            }
        }

        public void SaveRuntimeData()
        {
            ES3.Save(_unique.Id, _coterieEquipmentData);
        }

        public void LoadRuntimeData()
        {
            _coterieEquipmentData = ES3.Load<SerializableStringEquipmentDataDictionary>(_unique.Id);
        }
    }

    [Serializable]
    public class EquipmentData
    {
        [ReadOnly]
        public List<EquipmentSlot> equipmentSlots;

        public EquipmentData()
        {
            equipmentSlots = new();
            equipmentSlots.Add(new EquipmentSlot(ItemCategory.WEAPON));
            equipmentSlots.Add(new EquipmentSlot(ItemCategory.WEAPON));
            equipmentSlots.Add(new EquipmentSlot(ItemCategory.HEADWEAR));
            equipmentSlots.Add(new EquipmentSlot(ItemCategory.FACEWEAR));
            equipmentSlots.Add(new EquipmentSlot(ItemCategory.CLOTHING));
            equipmentSlots.Add(new EquipmentSlot(ItemCategory.OUTER_CLOTHING));
            equipmentSlots.Add(new EquipmentSlot(ItemCategory.TROUSERS));
            equipmentSlots.Add(new EquipmentSlot(ItemCategory.SHOES));
        }
    }

    [Serializable]
    public class SerializableStringEquipmentDataDictionary : SerializableDictionary<string, EquipmentData>
    {

    }
}
