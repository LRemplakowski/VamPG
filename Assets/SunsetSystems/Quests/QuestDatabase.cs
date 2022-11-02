using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace SunsetSystems.Journal
{
    [FilePath("Assets/SunsetSystems/Quests/QuestDatabase.asset", FilePathAttribute.Location.ProjectFolder)]
    public class QuestDatabase : ScriptableSingleton<QuestDatabase>
    {
        [SerializeField]
        private StringQuestDictionary _questRegistry = new();

        public bool RegisterQuest(Quest quest)
        {
            if (_questRegistry.ContainsKey(quest.ID))
            {
                Debug.LogError("Quest " + quest.ID + " is already registered in the database!");
                return false;
            }
            _questRegistry.Add(quest.ID, quest);
            return true;
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
