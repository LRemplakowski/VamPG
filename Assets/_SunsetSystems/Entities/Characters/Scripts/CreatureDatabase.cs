using NaughtyAttributes;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Party;
using SunsetSystems.Utils;
using System.Collections;
using System.Collections.Generic;
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
                Debug.LogWarning("CreatureConfig " + config.DatabaseID + " is already registered in the database!");
                return false;
            }
            _creatureRegistry.Add(config.DatabaseID, config);
            _accessorRegistry.Add(config.FullName.ToPascalCase(), config.DatabaseID);
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
            List<string> accessorKeysToDelete = new();
            foreach (string key in _accessorRegistry.Keys)
            {
                if (keysToDelete.Contains(_accessorRegistry[key]))
                    accessorKeysToDelete.Add(key);
            }
            accessorKeysToDelete.ForEach(key => _accessorRegistry.Remove(key.ToPascalCase()));
        }

        public void UnregisterConfig(CreatureConfig config)
        {
            if (_creatureRegistry.Remove(config.DatabaseID))
                Debug.Log("Removed quest " + config.DatabaseID + " from the database!");
            else
                Debug.LogError("Quest " + config.DatabaseID + " was not registered in the database!");
        }
    }
}
