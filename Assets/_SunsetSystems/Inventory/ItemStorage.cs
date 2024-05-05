using SunsetSystems.Inventory.Data;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using SunsetSystems.UI.Utils;
using UltEvents;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

namespace SunsetSystems.Inventory
{
    public class ItemStorage : SerializedMonoBehaviour
    {
        [Title("Data")]
        [SerializeField]
        private List<ItemCategory> _acceptedItemTypes = new();
        [OdinSerialize]
        private Dictionary<string, InventoryEntry> _contents = new();
        public List<InventoryEntry> Contents => _contents.Values.ToList();
        [Title("Events")]
        public UltEvent<InventoryEntry> OnItemAdded;
        public UltEvent<InventoryEntry> OnItemRemoved;

        private void OnValidate()
        {
            if (_contents == null)
                _contents = new();
            if (_acceptedItemTypes.Count() > 0)
            {
                List<string> keysToRemove = new();
                foreach (string key in _contents.Keys)
                {
                    if (_contents.TryGetValue(key, out var value) && _acceptedItemTypes.Contains(value._item.ItemCategory) is false)
                    {
                        keysToRemove.Add(key);
                    }
                }
                foreach (string key in keysToRemove)
                {
                    _contents.Remove(key);
                }
            }
        }

        public void AddItems(List<InventoryEntry> itemEntries)
        {
            itemEntries?.ForEach(entry => AddItem(entry));
        }

        [Button]
        public void AddItem(InventoryEntry itemEntry)
        {
            if (itemEntry._item == null)
                return;
            if (IsItemTypeAccepted(itemEntry._item.ItemCategory))
            {
                string entryID = itemEntry._item.DatabaseID;
                if (_contents.TryGetValue(entryID, out InventoryEntry storedItem))
                {
                    storedItem._stackSize += itemEntry._stackSize;
                    _contents[entryID] = storedItem;
                }
                else
                {
                    _contents[entryID] = itemEntry;
                }
                OnItemAdded?.InvokeSafe(itemEntry);
            }
        }

        private bool DoesInventoryContainItem(IBaseItem item)
        {
            return _contents.TryGetValue(item.DatabaseID, out _);
        }

        public bool TryRemoveItems(List<InventoryEntry> itemEntries)
        {
            return itemEntries.All(itemEntry => TryRemoveItem(itemEntry));
        }

        public bool TryRemoveItem(InventoryEntry entry)
        {
            if (_contents.TryGetValue(entry._item.DatabaseID, out InventoryEntry existing))
            {
                if (existing._stackSize >= entry._stackSize)
                {
                    existing._stackSize -= entry._stackSize;
                    if (existing._stackSize <= 0)
                        _contents.Remove(existing._item.DatabaseID);
                    else
                        _contents[existing._item.DatabaseID] = existing;
                    OnItemRemoved?.InvokeSafe(entry);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool IsItemTypeAccepted(ItemCategory itemType)
        {
            return _acceptedItemTypes.Count() == 0 || _acceptedItemTypes.Contains(itemType);
        }
    }

    [Serializable]
    public struct InventoryEntry : IGameDataProvider<InventoryEntry>
    {
        [NonSerialized, OdinSerialize, ES3Serializable]
        public IBaseItem _item;
        public int _stackSize;

        public InventoryEntry(IBaseItem item) : this(item, 1) { }

        public InventoryEntry(IBaseItem item, int stackSize)
        {
            this._item = item;
            this._stackSize = stackSize;
        }

        public InventoryEntry Data => this;
    }
}
