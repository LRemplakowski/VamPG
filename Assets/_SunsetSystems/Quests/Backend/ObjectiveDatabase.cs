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
            _objectiveRegistry.Values.ToList().ForEach(objective => _objectiveAccessorRegistry.Add(objective.ReadableID, objective.DatabaseID));
        }

        public bool TryGetQuest(string objectiveID, out Objective objective)
        {
            return _objectiveRegistry.TryGetValue(objectiveID, out objective);
        }

        public bool TryGetQuestByReadableID(string readableID, out Objective objective)
        {
            return TryGetQuest(_objectiveAccessorRegistry[readableID], out objective);
        }

        public bool RegisterObjective(Objective objective)
        {
            if (_objectiveRegistry.ContainsKey(objective.DatabaseID))
            {
                _objectiveAccessorRegistry = new();
                _objectiveRegistry.Values.ToList().ForEach(o => _objectiveAccessorRegistry.Add(o.ReadableID, o.DatabaseID));
                return false;
            }
            _objectiveRegistry.Add(objective.DatabaseID, objective);
            _objectiveAccessorRegistry = new();
            _objectiveRegistry.Values.ToList().ForEach(o => _objectiveAccessorRegistry.Add(o.ReadableID, o.DatabaseID));
            return true;
        }

        public void UnregisterObjective(Objective objective)
        {
            if (_objectiveRegistry.Remove(objective.DatabaseID))
            {
                _objectiveAccessorRegistry = new();
                _objectiveRegistry.Values.ToList().ForEach(q => _objectiveAccessorRegistry.Add(q.ReadableID, q.DatabaseID));
            }
        }
    }
}
