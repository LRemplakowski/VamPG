using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using SunsetSystems.Core.Database;
using SunsetSystems.Data;
using SunsetSystems.Inventory.Data;
using SunsetSystems.Persistence;
using UnityEngine;

namespace SunsetSystems.Inventory
{
    [RequireComponent(typeof(ItemStorage))]
    public class InventoryManager : SerializedMonoBehaviour, ISaveable, IResetable
    {
        public static InventoryManager Instance { get; private set; }

        [SerializeField]
        private ItemStorage _playerInventory;
        [SerializeField]
        private float _money;
       
        public ItemStorage PlayerInventory => Instance._playerInventory;

        public string DataKey => DataKeyConstants.INVENTORY_MANAGER_DATA_KEY;

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
            return new InventorySaveData(this);
        }

        public void InjectSaveData(object data)
        {
            if (data is not InventorySaveData saveData)
                return;
            foreach (var itemData in saveData.InventoryContents)
            {
                if (ItemDatabase.Instance.TryGetEntry(itemData.ItemID, out var item))
                {
                    _playerInventory.AddItem(new InventoryEntry(item, itemData.StackSize));
                }
            }
            _money = saveData.Money;
        }
    }

    [Serializable]
    public class InventorySaveData
    {
        public List<InventoryContentData> InventoryContents;
        public float Money;

        public InventorySaveData(InventoryManager manager)
        {
            foreach (var itemEntry in manager.PlayerInventory.Contents)
            {
                InventoryContents.Add(new() { ItemID = itemEntry._item.DatabaseID, StackSize = itemEntry._stackSize });
            }
            Money = manager.GetMoneyAmount();
        }

        public InventorySaveData()
        {

        }

        [Serializable]
        public struct InventoryContentData
        {
            public string ItemID;
            public int StackSize;
        }
    }
}
