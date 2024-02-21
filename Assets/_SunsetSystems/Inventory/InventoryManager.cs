using CleverCrow.Fluid.UniqueIds;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Inventory.Data;
using SunsetSystems.Persistence;
using SunsetSystems.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using SunsetSystems.Data;
using System.Linq;
using Sirenix.OdinInspector;

namespace SunsetSystems.Inventory
{
    [RequireComponent(typeof(ItemStorage)), RequireComponent(typeof(UniqueId))]
    public class InventoryManager : SerializedMonoBehaviour, ISaveable, IResetable
    {
        public static InventoryManager Instance { get; private set; }

        [SerializeField]
        private ItemStorage _playerInventory;
        [SerializeField]
        private float _money;
        private UniqueId _unique;
        public static ItemStorage PlayerInventory => Instance._playerInventory;

        public string DataKey => _unique.Id;

        public void ResetOnGameStart()
        {
            _playerInventory.Contents.Clear();
            _money = 0;
        }

        protected void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
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

        public void GiveItemToPlayer(InventoryEntry item)
        {
            PlayerInventory.AddItem(item);
        }

        public bool TakeItemFromPlayer(IBaseItem item, int amount)
        {
            return PlayerInventory.TryRemoveItem(new(item, amount));
        }

        public bool GetInventoryContainsItemWithReadableID(string itemID, out int count)
        {
            count = 0;
            var items = _playerInventory.Contents.FindAll(entry => entry._item.ReadableID == itemID);
            if (items != null && items.Count() > 0)
            {
                count = items.Count();
                return true;
            }
            else
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
            saveData.InventoryContents = _playerInventory.Contents ?? new();
            saveData.Money = _money;
            return saveData;
        }

        public void InjectSaveData(object data)
        {
            if (data is not InventorySaveData saveData)
                return;
            this._playerInventory.AddItems(saveData.InventoryContents);
            this._money = saveData.Money;
        }
    }

    [Serializable]
    public class InventorySaveData
    {
        public List<InventoryEntry> InventoryContents;
        public float Money;
    }
}
