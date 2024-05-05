using Sirenix.OdinInspector;
using SunsetSystems.Entities.Characters;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SunsetSystems.Entities
{
    [CreateAssetMenu(fileName = "Creature Database", menuName = "Entities/Creature Database")]
    public class CreatureDatabase : SerializedScriptableObject
    {
        [SerializeField]
        private Dictionary<string, CreatureConfig> _creatureRegistry = new();
        [SerializeField]
        private Dictionary<string, string> _accessorRegistry = new();
        public List<string> AccessorKeys => _accessorRegistry.Keys.ToList();

        public static CreatureDatabase Instance { get; private set; }

        public bool TryGetConfig(string accessorID, out CreatureConfig config)
        {
            accessorID ??= "";
            if (_accessorRegistry.ContainsKey(accessorID))
            {
                return _creatureRegistry.TryGetValue(_accessorRegistry[accessorID], out config);
            }
            else
            {
                return _creatureRegistry.TryGetValue(accessorID, out config);
            }
        }

        public bool RegisterConfig(CreatureConfig config)
        {
            if (_creatureRegistry.ContainsKey(config.DatabaseID))
            {
                _accessorRegistry = new();
                _creatureRegistry.Values.ToList().ForEach(c => _accessorRegistry.TryAdd(c.ReadableID, c.DatabaseID));
                return false;
            }
            _creatureRegistry.TryAdd(config.DatabaseID, config);
            _accessorRegistry = new();
            _creatureRegistry.Values.ToList().ForEach(c => _accessorRegistry.TryAdd(c.ReadableID, c.DatabaseID));
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
            return true;
        }

        public bool IsRegistered(CreatureConfig config)
        {
            return _creatureRegistry.ContainsKey(config.DatabaseID);
        }

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
            foreach (string key in _creatureRegistry.Keys)
            {
                if (_creatureRegistry[key] == null)
                    keysToDelete.Add(key);
            }
            keysToDelete.ForEach(key => _creatureRegistry.Remove(key));
            _accessorRegistry = new();
            _creatureRegistry.Values.ToList().ForEach(c => _accessorRegistry.TryAdd(c.ReadableID, c.DatabaseID));
        }

        public void UnregisterConfig(CreatureConfig config)
        {
            if (_creatureRegistry.Remove(config.DatabaseID))
            {
                _accessorRegistry = new();
                _creatureRegistry.Values.ToList().ForEach(c => _accessorRegistry.TryAdd(c.ReadableID, c.DatabaseID));
            }
        }
    }
}
