using SunsetSystems.Inventory.Data;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace SunsetSystems.Inventory
{
    public class ItemStorage : MonoBehaviour
    {
        [SerializeField]
        private List<ItemCategory> _acceptedItemTypes = new();
        [SerializeField]
        private List<InventoryEntry> _contents = new();
        public List<InventoryEntry> Contents => _contents;

        private void OnValidate()
        {
            if (_acceptedItemTypes.Count() > 0)
            {
                _contents
                    .FindAll(entry => entry != null && entry._item != null)
                    .FindAll(entry => !_acceptedItemTypes.Contains(entry._item.ItemCategory))
                    .ForEach(item => _contents.Remove(item));
            }
        }

        public void AddItems(List<InventoryEntry> itemEntries)
        {
            itemEntries.ForEach(entry => AddItem(entry));
        }

        public void AddItem(InventoryEntry itemEntry)
        {
            if (itemEntry == null || itemEntry._item == null)
                return;
            if (IsItemTypeAccepted(itemEntry._item.ItemCategory))
            {
                if (DoesInventoryContainItem(itemEntry._item))
                {
                    _contents.Find(existing => existing._item.ID.Equals(itemEntry._item.ID))._stackSize += itemEntry._stackSize;
                }
                else
                {
                    _contents.Add(itemEntry);
                }
            }
        }

        private bool DoesInventoryContainItem(BaseItem item)
        {
            return _contents.Any(entry => entry._item.ID.Equals(item.ID));
        }

        public bool TryRemoveItems(List<InventoryEntry> itemEntries)
        {
            return itemEntries.All(itemEntry => TryRemoveItem(itemEntry));
        }

        public bool TryRemoveItem(InventoryEntry entry)
        {
            if (DoesInventoryContainItem(entry._item))
            {
                InventoryEntry existing = _contents.Find(existingEntry => existingEntry._item.ID.Equals(entry._item.ID));
                if (existing._stackSize >= entry._stackSize)
                {
                    existing._stackSize -= entry._stackSize;
                    if (existing._stackSize <= 0)
                        _contents.Remove(existing);
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
    public class InventoryEntry
    {
        [SerializeField]
        public BaseItem _item;
        [SerializeField]
        public int _stackSize;

        public InventoryEntry(BaseItem item) : this(item, 1) { }

        public InventoryEntry(BaseItem item, int stackSize)
        {
            this._item = item;
            this._stackSize = stackSize;
        }
    }
}
