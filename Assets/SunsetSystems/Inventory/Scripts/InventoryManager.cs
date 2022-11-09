using CleverCrow.Fluid.UniqueIds;
using SunsetSystems.Entities.Characters;
using NaughtyAttributes;
using SunsetSystems.Inventory;
using SunsetSystems.Inventory.Data;
using SunsetSystems.Loading;
using SunsetSystems.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using SunsetSystems.Party;

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
        private StringEquipmentDataDictionary _coterieEquipmentData = new();
        private UniqueId _unique;

        public static event Action<Creature, EquipableItem> ItemEquipped, ItemUnequipped;

        protected override void Awake()
        {
            base.Awake();
            if (!_playerInventory)
                _playerInventory = GetComponent<ItemStorage>();
            if (!_playerInventory)
                _playerInventory = gameObject.AddComponent<ItemStorage>();
            _unique ??= GetComponent<UniqueId>();
        }

        private void OnEnable()
        {
            PartyManager.OnPartyMemberRecruited += AddCoterieMemberEquipment;
        }

        public static void AddCoterieMemberEquipment(string creatureID, CreatureData creatureData)
        {
            if (creatureData.useEquipmentPreset)
            {
                throw new NotImplementedException("Equipment presets not implemented!");
            }
            else
            {
                bool success = Instance._coterieEquipmentData.TryAdd(creatureID, EquipmentData.Initialize());
                if (!success)
                    Debug.LogWarning($"Trying to add coterie member equipment, but equipment data for {creatureID} already exists!");
            }
        }

        public static bool TryGetEquipmentData(Creature character, out EquipmentData data)
        {
            return Instance._coterieEquipmentData.TryGetValue(character.Data.FullName, out data);
        }

        public static bool TryEquipItemInSlot(Creature character, string slotID, EquipableItem item)
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

        public static bool TryUnequipItemFromSlot(Creature character, string slotID)
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
            InventorySaveData saveData = new();
            saveData.EquipmentData = _coterieEquipmentData;
            saveData.PlayerInventory = _playerInventory;
            ES3.Save(_unique.Id, saveData);
        }

        public void LoadRuntimeData()
        {
            InventorySaveData saveData = ES3.Load<InventorySaveData>(_unique.Id);
            this._coterieEquipmentData = saveData.EquipmentData;
            this._playerInventory = saveData.PlayerInventory;
        }

        private struct InventorySaveData
        {
            public StringEquipmentDataDictionary EquipmentData;
            public ItemStorage PlayerInventory;
        }
    }

    [Serializable]
    public struct EquipmentData
    {
        public const string SLOT_WEAPON_PRIMARY = "SLOT_WEAPON_PRIMARY";
        public const string SLOT_WEAPON_SECONDARY = "SLOT_WEAPON_SECONDARY";
        public const string SLOT_CHEST = "SLOT_CHEST";
        public const string SLOT_BOOTS = "SLOT_BOOTS";
        public const string SLOT_HANDS = "SLOT_HANDS";
        public const string SLOT_TRINKET = "SLOT_TRINKET";

        [ReadOnly]
        public StringEquipmentSlotDictionary equipmentSlots;

        public static EquipmentData Initialize()
        {
            EquipmentData data = new();
            data.equipmentSlots = new();
            data.equipmentSlots.Add(SLOT_WEAPON_PRIMARY, new EquipmentSlot(ItemCategory.WEAPON, "Primary Weapon", SLOT_WEAPON_PRIMARY));
            data.equipmentSlots.Add(SLOT_WEAPON_SECONDARY, new EquipmentSlot(ItemCategory.WEAPON, "Secondary Weapon", SLOT_WEAPON_SECONDARY));
            data.equipmentSlots.Add(SLOT_CHEST, new EquipmentSlot(ItemCategory.CLOTHING, "Chest", SLOT_CHEST));
            data.equipmentSlots.Add(SLOT_BOOTS, new EquipmentSlot(ItemCategory.SHOES, "Boots", SLOT_BOOTS));
            data.equipmentSlots.Add(SLOT_HANDS, new EquipmentSlot(ItemCategory.GLOVES, "Hands", SLOT_HANDS));
            data.equipmentSlots.Add(SLOT_TRINKET, new EquipmentSlot(ItemCategory.TRINKET, "Trinket", SLOT_TRINKET));
            return data;
        }
    }
}
