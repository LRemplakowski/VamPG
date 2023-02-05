using CleverCrow.Fluid.UniqueIds;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Inventory.Data;
using SunsetSystems.Loading;
using SunsetSystems.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using SunsetSystems.Party;
using SunsetSystems.Data;
using System.Web.UI.WebControls;
using System.Linq;

namespace SunsetSystems.Inventory
{
    [RequireComponent(typeof(ItemStorage)), RequireComponent(typeof(UniqueId))]
    public class InventoryManager : Singleton<InventoryManager>, ISaveable, IResetable
    {
        [SerializeField]
        private ItemStorage _playerInventory;
        [SerializeField]
        private float _money;
        public static ItemStorage PlayerInventory => Instance._playerInventory;
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

        protected override void Awake()
        {
            base.Awake();
            if (!_playerInventory)
                _playerInventory = GetComponent<ItemStorage>();
            if (!_playerInventory)
                _playerInventory = gameObject.AddComponent<ItemStorage>();
            ISaveable.RegisterSaveable(this);
            _unique ??= GetComponent<UniqueId>();
        }

        private void OnDestroy()
        {
            ISaveable.UnregisterSaveable(this);
        }

        public static void AddCoterieMemberEquipment(string creatureID, CreatureData creatureData)
        {
            bool success = Instance._coterieEquipmentData.TryAdd(creatureID, creatureData.Equipment);
            if (!success)
                Debug.LogWarning($"Trying to add coterie member equipment, but equipment data for {creatureID} already exists!");
            else
                Debug.Log($"Successfully added equipment entry for coterie member with ID {creatureID}");
        }

        public static bool TryGetEquipmentData(string characterKey, out EquipmentData data)
        {
            return Instance._coterieEquipmentData.TryGetValue(characterKey, out data);
        }

        public static bool TryEquipItemInSlot(string characterID, string slotID, EquipableItem item)
        {
            if (Instance._coterieEquipmentData.TryGetValue(characterID, out EquipmentData equipmentData) == false)
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
                        Debug.LogError("Could not unequip item " + slot.GetEquippedItem().ItemName + " from slot " + slotID + " for creature " + characterID + "!");
                        return false;
                    }
                }
                bool success = slot.TryEquipItem(item);
                if (success)
                {
                    Debug.Log("Item equipped successfuly!");
                    PlayerInventory.TryRemoveItem(new(item));
                    equipmentData.EquipmentSlots[slotID] = slot;
                    Instance._coterieEquipmentData[characterID] = equipmentData;
                    CreatureData data = PartyManager.Instance.GetPartyMemberByID(characterID).Data;
                    data.Equipment = Instance._coterieEquipmentData[characterID];
                    PartyManager.Instance.GetPartyMemberByID(characterID).Data = data;
                    ItemEquipped?.Invoke(characterID);
                }
                else
                {
                    Debug.LogError($"Failed to equip item {item.ItemName} in slot {slot.Name}!");
                }
                return success;
            }
            catch (IndexOutOfRangeException e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public static bool TryEquipItem(string characterKey, EquipableItem item)
        {
            List<string> slotIDs = EquipmentData.GetSlotIDsFromItemCategory(item.ItemCategory);
            if (slotIDs.Count <= 0)
                return false;
            return TryEquipItemInSlot(characterKey, slotIDs[0], item);
        }

        public static bool TryUnequipItemFromSlot(string characterID, string slotID)
        {
            if (Instance._coterieEquipmentData.TryGetValue(characterID, out EquipmentData equipmentData) == false)
                return false;
            try
            {
                EquipmentSlot slot = equipmentData.EquipmentSlots[slotID];
                EquipableItem item = slot.GetEquippedItem();
                bool success = slot.TryUnequipItem(item);
                if (success)
                {
                    PlayerInventory.AddItem(new(item));
                    equipmentData.EquipmentSlots[slot.ID] = slot;
                    Instance._coterieEquipmentData[characterID] = equipmentData;
                    CreatureData data = PartyManager.Instance.GetPartyMemberByID(characterID).Data;
                    data.Equipment = Instance._coterieEquipmentData[characterID];
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

        public void AddMoney(float value)
        {
            _money += value;
        }

        public bool TryRemoveMoney(float value)
        {
            if (value > _money)
                return false;
            _money -= value;
            return true;
        }

        public float GetMoneyAmount()
        {
            return _money;
        }

        public void SetMoney(float value)
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
        public float Money;
    }
}
