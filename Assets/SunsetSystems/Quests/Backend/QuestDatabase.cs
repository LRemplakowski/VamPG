using NaughtyAttributes;
using SunsetSystems.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Journal
{
    [CreateAssetMenu(fileName = "Quest Database", menuName = "Sunset Journal/Database")]
    public class QuestDatabase : ScriptableObjectSingleton<QuestDatabase>
    {
        [SerializeField]
        private StringQuestDictionary _questRegistry = new();

        public bool TryGetQuest(string questID, out Quest quest)
        {
            return _questRegistry.TryGetValue(questID, out quest);
        }

        public bool RegisterQuest(Quest quest)
        {
            if (_questRegistry.ContainsKey(quest.ID))
            {
                Debug.LogWarning("Quest " + quest.ID + " is already registered in the database!");
                return false;
            }
            _questRegistry.Add(quest.ID, quest);
            return true;
        }

        public bool IsRegistered(Quest quest)
        {
            return _questRegistry.ContainsKey(quest.ID);
        }    

        private void OnValidate()
        {
            List<string> keysToDelete = new();
            foreach (string key in _questRegistry.Keys)
            {
                if (_questRegistry[key] == null)
                    keysToDelete.Add(key);
            }
            keysToDelete.ForEach(key => _questRegistry.Remove(key));
        }

        public void UnregisterQuest(Quest quest)
        {
            if (_questRegistry.Remove(quest.ID))
                Debug.Log("Removed quest " + quest.ID + " from the database!");
            else
                Debug.LogError("Quest " + quest.ID + " was not registered in the database!");
        }
    }
}
