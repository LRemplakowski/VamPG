using CleverCrow.Fluid.UniqueIds;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Inventory.Data;
using SunsetSystems.LevelManagement;
using System;
using System.Collections.Generic;
using UnityEngine;
using SunsetSystems.Data;
using System.Linq;

namespace SunsetSystems.Inventory
{
    [RequireComponent(typeof(ItemStorage)), RequireComponent(typeof(UniqueId))]
    public class InventoryManager : MonoBehaviour, ISaveable, IResetable, IInventoryManager
    {
        [SerializeField]
        private ItemStorage _playerInventory;
        [SerializeField]
        private int _money;
        [SerializeField, ES3Serializable]
        private StringEquipmentDataDictionary _coterieEquipmentData = new();
        private UniqueId _unique;
        public string DataKey => _unique.Id;

        public static event Action<string> ItemEquipped, ItemUnequipped;

        public void ResetOnGameStart()
        {
            _playerInventory.Contents.Clear();
            _money = 0;
            _coterieEquipmentData = new();
        }

        protected void Awake()
        {
            if (!_playerInventory)
                _playerInventory = GetComponent<ItemStorage>();
            if (!_playerInventory)
                _playerInventory = gameObject.AddComponent<ItemStorage>();
            _unique ??= GetComponent<UniqueId>();
            ISaveable.RegisterSaveable(this);
        }

        private void OnDestroy()
        {
            ISaveable.UnregisterSaveable(this);
        }

        public void AddCoterieMemberEquipment(string creatureID, CreatureData creatureData)
        {
            bool success = _coterieEquipmentData.TryAdd(creatureID, creatureData.Equipment);
            if (!success)
                Debug.LogWarning($"Trying to add coterie member equipment, but equipment data for {creatureID} already exists!");
            else
                Debug.Log($"Successfully added equipment entry for coterie member with ID {creatureID}");
        }

        public bool TryGetEquipmentData(string characterKey, out EquipmentData data)
        {
            return _coterieEquipmentData.TryGetValue(characterKey, out data);
        }

        public bool TryEquipItemInSlot(string characterID, string slotID, EquipableItem item)
        {
            if (_coterieEquipmentData.TryGetValue(characterID, out EquipmentData equipmentData) == false)
            {
                Debug.LogError($"Could not find equipment data for creater with ID {characterID}! Is {characterID} registered as party member?");
                return false;
            }
            try
            {
                EquipmentSlot slot = equipmentData.EquipmentSlots[slotID];
                if (slot.GetEquippedItem() != null)
                {
                    if (TryUnequipItemFromSlot(characterID, slotID) == false)
                    {
                        Debug.LogError("Could not unequip item " + slot.GetEquippedItem().ReadableID + " from slot " + slotID + " for creature " + characterID + "!");
                        return false;
                    }
                }
                bool success = slot.TryEquipItem(item);
                if (success)
                {
                    Debug.Log("Item equipped successfuly!");
                    _playerInventory.TryRemoveItem(new(item));
                    equipmentData.EquipmentSlots[slotID] = slot;
                    _coterieEquipmentData[characterID] = equipmentData;
                    ItemEquipped?.Invoke(characterID);
                }
                else
                {
                    Debug.LogError($"Failed to equip item {item.ReadableID} in slot {slot.Name}!");
                }
                return success;
            }
            catch (IndexOutOfRangeException e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool TryEquipItem(string characterKey, EquipableItem item)
        {
            List<string> slotIDs = EquipmentData.GetSlotIDsFromItemCategory(item.ItemCategory);
            if (slotIDs.Count <= 0)
                return false;
            return TryEquipItemInSlot(characterKey, slotIDs[0], item);
        }

        public bool TryUnequipItemFromSlot(string characterID, string slotID)
        {
            if (_coterieEquipmentData.TryGetValue(characterID, out EquipmentData equipmentData) == false)
                return false;
            try
            {
                EquipmentSlot slot = equipmentData.EquipmentSlots[slotID];
                EquipableItem item = slot.GetEquippedItem();
                bool success = slot.TryUnequipItem(item);
                if (success)
                {
                    _playerInventory.AddItem(new(item));
                    equipmentData.EquipmentSlots[slot.ID] = slot;
                    _coterieEquipmentData[characterID] = equipmentData;
                    ItemUnequipped?.Invoke(characterID);
                }
                return success;
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
        }

        public void TransferItem(ItemStorage from, ItemStorage to, InventoryEntry item)
        {
            if (from.TryRemoveItem(item))
            {
                to.AddItem(item);
            }
        }

        public void AddEntryToPlayerInventory(InventoryEntry entry)
        {
            _playerInventory.AddItem(entry);
        }

        public List<InventoryEntry> GetPlayerInventoryContents() => new(_playerInventory.Contents);

        public void AddMoney(int value)
        {
            _money += value;
        }

        public bool TryRemoveMoney(int value)
        {
            if (value > _money)
                return false;
            _money -= value;
            return true;
        }

        public int GetMoney()
        {
            return _money;
        }

        public void SetMoney(int value)
        {
            _money = value;
        }

        public object GetSaveData()
        {
            InventorySaveData saveData = new();
            saveData.EquipmentData = new(_coterieEquipmentData);
            saveData.PlayerInventory = _playerInventory;
            saveData.Money = _money;
            return saveData;
        }

        public void InjectSaveData(object data)
        {
            InventorySaveData saveData = data as InventorySaveData;
            this._coterieEquipmentData = new();
            this._coterieEquipmentData.Concat(saveData.EquipmentData);
            this._playerInventory = saveData.PlayerInventory;
            this._money = saveData.Money;
        }
    }

    [Serializable]
    public class InventorySaveData
    {
        public Dictionary<string, EquipmentData> EquipmentData;
        public ItemStorage PlayerInventory;
        public int Money;
    }
}
