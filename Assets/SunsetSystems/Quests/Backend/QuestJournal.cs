using CleverCrow.Fluid.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SunsetSystems.Journal
{
    public class QuestJournal : Singleton<QuestJournal>
    {
        [SerializeField]
        private StringQuestDictionary _activeQuests = new(), _completedQuests = new();

        public List<Quest> ActiveQuests => _activeQuests.Values.ToList();
        public List<Quest> MainQuests => _activeQuests.Select(kv => kv.Value).Where(quest => quest.Data.Category.Equals(QuestCategory.Main)).ToList();
        public List<Quest> SideQuests => _activeQuests.Select(kv => kv.Value).Where(quest => quest.Data.Category.Equals(QuestCategory.Side)).ToList();
        public List<Quest> CaseQuests => _activeQuests.Select(kv => kv.Value).Where(quest => quest.Data.Category.Equals(QuestCategory.Case)).ToList();
        public List<Quest> CompletedQuests => _completedQuests.Values.ToList();

        public bool BeginQuest(string questID)
        {
            if (_completedQuests.ContainsKey(questID))
            {
                Debug.Log("Quest " + _completedQuests[questID].Data.Name + " has already been completed!");
                return false;
            }
            if (_activeQuests.ContainsKey(questID))
            {
                Debug.Log("Quest " + _activeQuests[questID].Data.Name + " has already been started!");
                return false;
            }
            if (QuestDatabase.Instance.TryGetQuest(questID, out Quest quest))
            {
                _activeQuests.Add(questID, quest);
                quest.Begin();
                return true;
            }
            else
            {
                Debug.LogError("There is no quest with ID " + questID + " in the database!");
                return false;
            }
        }

        public bool CompleteQuest(string questID)
        {
            if (_activeQuests.ContainsKey(questID))
            {
                Quest quest = _activeQuests[questID];
                if (_completedQuests.TryAdd(questID, quest))
                {
                    _activeQuests.Remove(questID);
                    quest.Complete();
                    return true;
                }
                return false;
            }
            return false;
        }
    }
}
