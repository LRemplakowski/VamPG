using NaughtyAttributes;
using SunsetSystems.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SunsetSystems.Journal
{
    [CreateAssetMenu(fileName = "Quest Database", menuName = "Sunset Journal/Database")]
    public class QuestDatabase : ScriptableObjectSingleton<QuestDatabase>
    {
        [SerializeField]
        private StringQuestDictionary _questRegistry = new();
        [SerializeField]
        private StringStringDictionary _questAccessorRegistry = new();

        public bool TryGetQuest(string questID, out Quest quest)
        {
            return _questRegistry.TryGetValue(questID, out quest);
        }

        public bool TryGetQuestByReadableID(string readableID, out Quest quest)
        {
            return TryGetQuest(_questAccessorRegistry[readableID], out quest);
        }    

        public bool RegisterQuest(Quest quest)
        {
            if (_questRegistry.ContainsKey(quest.ID))
            {
                _questAccessorRegistry = new();
                _questRegistry.Values.ToList().ForEach(q => _questAccessorRegistry.Add(q.ReadableID, q.ID));
                Debug.LogWarning("Quest " + quest.ReadableID + " is already registered in the database!");
                return false;
            }
            _questRegistry.Add(quest.ID, quest);
            _questAccessorRegistry = new();
            _questRegistry.Values.ToList().ForEach(q => _questAccessorRegistry.Add(q.ReadableID, q.ID));
            return true;
        }

        public bool IsRegistered(Quest quest)
        {
            return _questRegistry.ContainsKey(quest.ID);
        }

        [ContextMenu("Force Become Instance")]
        public void ForceBecomeInstance()
        {
            _instance = this;
        }

        protected override void OnValidate()
        {
            List<string> keysToDelete = new();
            foreach (string key in _questRegistry.Keys)
            {
                if (_questRegistry[key] == null)
                    keysToDelete.Add(key);
            }
            keysToDelete.ForEach(key => _questRegistry.Remove(key));
            _questAccessorRegistry = new();
            _questRegistry.Values.ToList().ForEach(q => _questAccessorRegistry.Add(q.ReadableID, q.ID));
        }

        public void UnregisterQuest(Quest quest)
        {
            if (_questRegistry.Remove(quest.ID))
            {
                _questAccessorRegistry = new();
                _questRegistry.Values.ToList().ForEach(q => _questAccessorRegistry.Add(q.ReadableID, q.ID));
            }
        }
    }
}
