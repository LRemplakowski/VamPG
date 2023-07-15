using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SunsetSystems.Journal
{
    [CreateAssetMenu(fileName = "Quest Database", menuName = "Sunset Journal/Quest Database")]
    public class QuestDatabase : SerializedScriptableObject
    {
        [SerializeField]
        private Dictionary<string, Quest> _questRegistry = new();
        [SerializeField]
        private Dictionary<string, string> _questAccessorRegistry = new();

        public static QuestDatabase Instance { get; private set; }

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
            if (quest.ID == null)
            {
                Debug.LogError($"Quest {quest} has null ID string!");
            }
            if (_questRegistry.ContainsKey(quest.ID))
            {
                _questAccessorRegistry = new();
                _questRegistry.Values.ToList().ForEach(q => _questAccessorRegistry.TryAdd(q.ReadableID, q.ID));
                return false;
            }
            _questRegistry.TryAdd(quest.ID, quest);
            _questAccessorRegistry = new();
            _questRegistry.Values.ToList().ForEach(q => _questAccessorRegistry.TryAdd(q.ReadableID, q.ID));
            return true;
        }

        public bool IsRegistered(Quest quest)
        {
            return _questRegistry.ContainsKey(quest.ID);
        }
        
        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            Instance = this;
        }

        private void OnValidate()
        {
            Instance = this;
            List<string> keysToDelete = new();
            foreach (string key in _questRegistry.Keys)
            {
                if (_questRegistry[key] == null)
                    keysToDelete.Add(key);
            }
            keysToDelete.ForEach(key => _questRegistry.Remove(key));
            _questAccessorRegistry = new();
            _questRegistry.Values.ToList().ForEach(q => _questAccessorRegistry.TryAdd(q.ReadableID, q.ID));
        }

        public void UnregisterQuest(Quest quest)
        {
            if (_questRegistry.Remove(quest.ID))
            {
                _questAccessorRegistry = new();
                _questRegistry.Values.ToList().ForEach(q => _questAccessorRegistry.TryAdd(q.ReadableID, q.ID));
            }
        }
    }
}
