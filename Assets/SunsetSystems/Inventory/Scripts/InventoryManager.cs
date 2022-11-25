using CleverCrow.Fluid.UniqueIds;
using SunsetSystems.Entities.Characters;
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

        public static event Action<string> ItemEquipped, ItemUnequipped;

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

        public static bool TryGetEquipmentData(string characterKey, out EquipmentData data)
        {
            return Instance._coterieEquipmentData.TryGetValue(characterKey, out data);
        }

        public static bool TryEquipItemInSlot(string characterID, string slotID, EquipableItem item)
        {
            if (Instance._coterieEquipmentData.TryGetValue(characterID, out EquipmentData equipmentData) == false)
                return false;
            try
            {
                EquipmentSlot slot = equipmentData.equipmentSlots[slotID];
                if (slot.GetEquippedItem() != null)
                {
                    if (TryUnequipItemFromSlot(characterID, slotID) == false)
                    {
                        Debug.LogError("Could not unequip item " + slot.GetEquippedItem().ItemName + " from slot " + slotID + " for creature " + characterID + "!");
                        return false;
                    }
                }
                bool success = slot.TryEquipItem(item);
                if (success)
                {
                    PlayerInventory.TryRemoveItem(new(item));
                    equipmentData.equipmentSlots[slotID] = slot;
                    Instance._coterieEquipmentData[characterID] = equipmentData;
                    CreatureData data = PartyManager.Instance.GetPartyMemberByID(characterID).Data;
                    data.equipment = Instance._coterieEquipmentData[characterID];
                    PartyManager.Instance.GetPartyMemberByID(characterID).Data = data;
                    ItemEquipped?.Invoke(characterID);
                }
                return success;
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
        }

        public static bool TryEquipItem(string characterKey, EquipableItem item)
        {
            if (TryGetEquipmentData(characterKey, out EquipmentData data))
            {
                List<string> slotIDs = EquipmentData.GetSlotIDsFromItemCategory(item.ItemCategory);
                if (slotIDs.Count <= 0)
                    return false;
                return TryEquipItemInSlot(characterKey, slotIDs[0], item);
            }
            return false;
        }

        public static bool TryUnequipItemFromSlot(string characterID, string slotID)
        {
            if (Instance._coterieEquipmentData.TryGetValue(characterID, out EquipmentData equipmentData) == false)
                return false;
            try
            {
                EquipmentSlot slot = equipmentData.equipmentSlots[slotID];
                EquipableItem item = slot.GetEquippedItem();
                bool success = slot.TryUnequipItem(item);
                if (success)
                {
                    PlayerInventory.AddItem(new(item));
                    equipmentData.equipmentSlots[slot.ID] = slot;
                    Instance._coterieEquipmentData[characterID] = equipmentData;
                    CreatureData data = PartyManager.Instance.GetPartyMemberByID(characterID).Data;
                    data.equipment = Instance._coterieEquipmentData[characterID];
                    PartyManager.Instance.GetPartyMemberByID(characterID).Data = data;
                    ItemUnequipped?.Invoke(characterID);
                }
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
}
