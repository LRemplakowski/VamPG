using CleverCrow.Fluid.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Journal
{
    public class QuestJournal : Singleton<QuestJournal>
    {
        private StringQuestDictionary _activeQuests = new(), _completedQuests = new();

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
