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
            _contents ??= new();
            if (_acceptedItemTypes.Count() > 0)
            {
                List<string> keysToRemove = new();
                foreach (string key in _contents.Keys)
                {
                    if (_contents.TryGetValue(key, out var value) && _acceptedItemTypes.Contains(value.ItemReference.ItemCategory) is false)
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

        public bool AddItems(List<InventoryEntry> itemEntries)
        {
            bool success = true;
            itemEntries?.ForEach(entry => success &= AddItem(entry));
            return success;
        }

        [Button]
        public bool AddItem(InventoryEntry itemEntry)
        {
            if (itemEntry.ItemReference == null)
                return false;
            if (IsItemTypeAccepted(itemEntry.ItemReference.ItemCategory))
            {
                string entryID = itemEntry.ItemReference.DatabaseID;
                if (_contents.TryGetValue(entryID, out InventoryEntry storedItem))
                {
                    storedItem.StackSize += itemEntry.StackSize;
                    _contents[entryID] = storedItem;
                }
                else
                {
                    _contents[entryID] = itemEntry;
                }
                OnItemAdded?.InvokeSafe(itemEntry);
                return true;
            }
            return false;
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
            if (_contents.TryGetValue(entry.ItemReference.DatabaseID, out InventoryEntry existing))
            {
                if (existing.StackSize >= entry.StackSize)
                {
                    existing.StackSize -= entry.StackSize;
                    if (existing.StackSize <= 0)
                        _contents.Remove(existing.ItemReference.DatabaseID);
                    else
                        _contents[existing.ItemReference.DatabaseID] = existing;
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
        public IBaseItem ItemReference;
        public int StackSize;

        public InventoryEntry(IBaseItem item) : this(item, 1) { }

        public InventoryEntry(IBaseItem item, int stackSize)
        {
            this.ItemReference = item;
            this.StackSize = stackSize;
        }

        public InventoryEntry Data => this;
    }
}
