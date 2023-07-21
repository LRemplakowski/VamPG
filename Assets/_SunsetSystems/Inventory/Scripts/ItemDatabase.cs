using SunsetSystems.Inventory.Data;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SunsetSystems
{
    [CreateAssetMenu(fileName = "New Item Database", menuName = "Sunset Inventory/Item Database")]
    public class ItemDatabase : ScriptableObject
    {
        [SerializeField]
        private Dictionary<string, BaseItem> _objectiveRegistry = new();
        [SerializeField]
        private Dictionary<string, string> _objectiveAccessorRegistry = new();

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
            foreach (string key in _objectiveRegistry.Keys)
            {
                if (_objectiveRegistry[key] == null)
                    keysToDelete.Add(key);
            }
            keysToDelete.ForEach(key => _objectiveRegistry.Remove(key));
            _objectiveAccessorRegistry = new();
            _objectiveRegistry.Values.ToList().ForEach(baseItem => _objectiveAccessorRegistry.Add(baseItem.ReadableID, baseItem.DatabaseID));
        }

        public bool TryGetEntry(string objectiveID, out BaseItem baseItem)
        {
            return _objectiveRegistry.TryGetValue(objectiveID, out baseItem);
        }

        public bool TryGetEntryByReadableID(string readableID, out BaseItem baseItem)
        {
            return TryGetEntry(_objectiveAccessorRegistry[readableID], out baseItem);
        }

        public bool Register(BaseItem baseItem)
        {
            if (_objectiveRegistry.ContainsKey(baseItem.DatabaseID))
            {
                _objectiveAccessorRegistry = new();
                _objectiveRegistry.Values.ToList().ForEach(item => _objectiveAccessorRegistry.Add(item.ReadableID, item.DatabaseID));
                return false;
            }
            _objectiveRegistry.Add(baseItem.DatabaseID, baseItem);
            _objectiveAccessorRegistry = new();
            _objectiveRegistry.Values.ToList().ForEach(item => _objectiveAccessorRegistry.Add(item.ReadableID, item.DatabaseID));
            return true;
        }

        public void Unregister(BaseItem baseItem)
        {
            if (_objectiveRegistry.Remove(baseItem.DatabaseID))
            {
                _objectiveAccessorRegistry = new();
                _objectiveRegistry.Values.ToList().ForEach(item => _objectiveAccessorRegistry.Add(item.ReadableID, item.DatabaseID));
            }
        }
    }
}
