using Sirenix.OdinInspector;
using SunsetSystems.Inventory.Data;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SunsetSystems.Core.Database
{
    [CreateAssetMenu(fileName = "New Item Database", menuName = "Sunset Inventory/Item Database")]
    public class ItemDatabase : SerializedScriptableObject
    {
        [SerializeField]
        private Dictionary<string, IBaseItem> _itemRegistry = new();
        [SerializeField]
        private Dictionary<string, string> _itemAccessorRegistry = new();

        public static ItemDatabase Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            Instance = this;
        }

        protected void OnValidate()
        {
            Instance = this;
            List<string> keysToDelete = new();
            foreach (string key in _itemRegistry.Keys)
            {
                if (_itemRegistry[key] == null)
                    keysToDelete.Add(key);
            }
            keysToDelete.ForEach(key => _itemRegistry.Remove(key));
            _itemAccessorRegistry = new();
            _itemRegistry.Values.ToList().ForEach(baseItem => _itemAccessorRegistry.Add(baseItem.Name, baseItem.DatabaseID));
        }

        public bool TryGetEntry(string itemID, out IBaseItem baseItem)
        {
            return _itemRegistry.TryGetValue(itemID, out baseItem);
        }

        public bool TryGetEntryByReadableID(string readableID, out IBaseItem baseItem)
        {
            return TryGetEntry(_itemAccessorRegistry[readableID], out baseItem);
        }

        public bool Register(BaseItem baseItem)
        {
            if (_itemRegistry.ContainsKey(baseItem.DatabaseID))
            {
                _itemAccessorRegistry = new();
                _itemRegistry.Values.ToList().ForEach(item => _itemAccessorRegistry.Add(item.Name, item.DatabaseID));
                return false;
            }
            _itemRegistry.Add(baseItem.DatabaseID, baseItem);
            _itemAccessorRegistry = new();
            _itemRegistry.Values.ToList().ForEach(item => _itemAccessorRegistry.Add(item.Name, item.DatabaseID));
            return true;
        }

        public void Unregister(BaseItem baseItem)
        {
            if (_itemRegistry.Remove(baseItem.DatabaseID))
            {
                _itemAccessorRegistry = new();
                _itemRegistry.Values.ToList().ForEach(item => _itemAccessorRegistry.Add(item.Name, item.DatabaseID));
            }
        }
    }
}
