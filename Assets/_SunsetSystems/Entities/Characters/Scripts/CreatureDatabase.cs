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
    public class CreatureDatabase : ScriptableObjectSingleton<CreatureDatabase>
    {
        [SerializeField]
        private StringCreatureConfigDictionary _creatureRegistry = new();
        [SerializeField]
        private StringStringDictionary _accessorRegistry = new();

        public bool TryGetConfig(string creatureName, out CreatureConfig config)
        {
            config = null;
            creatureName = creatureName.ToPascalCase();
            if (_accessorRegistry.ContainsKey(creatureName))
            {
                return _creatureRegistry.TryGetValue(_accessorRegistry[creatureName], out config);
            }
            return false;
        }

        public bool RegisterConfig(CreatureConfig config)
        {
            if (_creatureRegistry.ContainsKey(config.DatabaseID))
            {
                _accessorRegistry = new();
                _creatureRegistry.Values.ToList().ForEach(c => _accessorRegistry.Add(c.FullName.ToPascalCase(), c.DatabaseID));
                return false;
            }
            _creatureRegistry.Add(config.DatabaseID, config);
            _accessorRegistry = new();
            _creatureRegistry.Values.ToList().ForEach(c => _accessorRegistry.Add(c.FullName.ToPascalCase(), c.DatabaseID));
            return true;
        }

        public bool IsRegistered(CreatureConfig config)
        {
            return _creatureRegistry.ContainsKey(config.DatabaseID);
        }

        protected override void OnValidate()
        {
            List<string> keysToDelete = new();
            foreach (string key in _creatureRegistry.Keys)
            {
                if (_creatureRegistry[key] == null)
                    keysToDelete.Add(key);
            }
            keysToDelete.ForEach(key => _creatureRegistry.Remove(key));
            _accessorRegistry = new();
            _creatureRegistry.Values.ToList().ForEach(c => _accessorRegistry.Add(c.FullName.ToPascalCase(), c.DatabaseID));
        }

        public void UnregisterConfig(CreatureConfig config)
        {
            if (_creatureRegistry.Remove(config.DatabaseID))
            {
                _accessorRegistry = new();
                _creatureRegistry.Values.ToList().ForEach(c => _accessorRegistry.Add(c.FullName.ToPascalCase(), c.DatabaseID));
            }
        }
    }
}
