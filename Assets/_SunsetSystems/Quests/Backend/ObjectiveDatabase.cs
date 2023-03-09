using SunsetSystems.Journal;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SunsetSystems
{
    [CreateAssetMenu(fileName = "Objective Database", menuName = "Sunset Journal/Objective Database")]
    public class ObjectiveDatabase : ScriptableObject
    {
        [SerializeField]
        private StringObjectiveDictionary _objectiveRegistry = new();
        [SerializeField]
        private StringStringDictionary _objectiveAccessorRegistry = new();

        public static ObjectiveDatabase Instance { get; private set; }

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
            _objectiveRegistry.Values.ToList().ForEach(objective => _objectiveAccessorRegistry.TryAdd(objective.ReadableID, objective.DatabaseID));
        }

        public bool TryGetEntry(string objectiveID, out Objective objective)
        {
            return _objectiveRegistry.TryGetValue(objectiveID, out objective);
        }

        public bool TryGetEntryByReadableID(string readableID, out Objective objective)
        {
            return TryGetEntry(_objectiveAccessorRegistry[readableID], out objective);
        }

        public bool Register(Objective objective)
        {
            if (_objectiveRegistry.ContainsKey(objective.DatabaseID))
            {
                _objectiveAccessorRegistry = new();
                _objectiveRegistry.Values.ToList().ForEach(o => _objectiveAccessorRegistry.TryAdd(o.ReadableID, o.DatabaseID));
                return false;
            }
            _objectiveRegistry.Add(objective.DatabaseID, objective);
            _objectiveAccessorRegistry = new();
            _objectiveRegistry.Values.ToList().ForEach(o => _objectiveAccessorRegistry.TryAdd(o.ReadableID, o.DatabaseID));
            return true;
        }

        public void Unregister(Objective objective)
        {
            if (_objectiveRegistry.Remove(objective.DatabaseID))
            {
                _objectiveAccessorRegistry = new();
                _objectiveRegistry.Values.ToList().ForEach(q => _objectiveAccessorRegistry.TryAdd(q.ReadableID, q.DatabaseID));
            }
        }
    }
}
