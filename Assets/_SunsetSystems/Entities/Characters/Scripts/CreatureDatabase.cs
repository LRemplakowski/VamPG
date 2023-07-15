using NaughtyAttributes;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Party;
using SunsetSystems.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SunsetSystems.Entities
{
    [CreateAssetMenu(fileName = "Creature Database", menuName = "Entities/Creature Database")]
    public class CreatureDatabase : ScriptableObject
    {
        [SerializeField]
        private StringCreatureConfigDictionary _creatureRegistry = new();
        [SerializeField]
        private StringStringDictionary _accessorRegistry = new();
        public List<string> AccessorKeys => _accessorRegistry.Keys.ToList();

        public static CreatureDatabase Instance { get; private set; }

        public bool TryGetConfig(string accessorID, out CreatureConfig config)
        {
            config = null;
            accessorID ??= "";
            if (_accessorRegistry.ContainsKey(accessorID))
            {
                return _creatureRegistry.TryGetValue(_accessorRegistry[accessorID], out config);
            }
            return false;
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
