using CleverCrow.Fluid.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SunsetSystems.Journal
{
    public class QuestJournal : Singleton<QuestJournal>
    {
        [SerializeField]
        private StringQuestDictionary _activeQuests = new(), _completedQuests = new();
        private Dictionary<string, Objective> _currentObjectives = new();
        [SerializeField]
        private List<Quest> _trackedQuests = new();

        public List<Quest> ActiveQuests => _activeQuests.Values.ToList();
        public List<Quest> MainQuests => _activeQuests.Select(kv => kv.Value).Where(quest => quest.QuestData.Category.Equals(QuestCategory.Main)).ToList();
        public List<Quest> SideQuests => _activeQuests.Select(kv => kv.Value).Where(quest => quest.QuestData.Category.Equals(QuestCategory.Side)).ToList();
        public List<Quest> CaseQuests => _activeQuests.Select(kv => kv.Value).Where(quest => quest.QuestData.Category.Equals(QuestCategory.Case)).ToList();
        public List<Quest> CompletedQuests => _completedQuests.Values.ToList();

        public static event Action<List<Quest>> OnTrackedQuestsChanged;

        private void OnEnable()
        {
            Quest.QuestStarted += OnQuestStarted;
            Quest.QuestCompleted += OnQuestCompleted;
            Quest.ObjectiveChanged += OnQuestObjectiveChanged;
        }

        private void OnDisable()
        {
            Quest.QuestStarted -= OnQuestStarted;
            Quest.QuestCompleted -= OnQuestCompleted;
            Quest.ObjectiveChanged -= OnQuestObjectiveChanged;
        }

        private void OnQuestStarted(Quest quest)
        {
            _activeQuests.Add(quest.ID, quest);
            _trackedQuests.Add(quest);
            _currentObjectives.Add(quest.ID, quest.QuestData.FirstObjective);
            OnTrackedQuestsChanged?.Invoke(_trackedQuests);
        }

        private void OnQuestObjectiveChanged(Quest quest, Objective objective)
        {
            if (objective != null && quest != null)
            {
                if (objective.NextObjective == null)
                    // Do nothing, quest will finish and handle cleanup
                    return;
                if (_currentObjectives.ContainsKey(quest.ID))
                    _currentObjectives[quest.ID] = objective.NextObjective;
                else
                    _currentObjectives.Add(quest.ID, objective);
                OnTrackedQuestsChanged?.Invoke(_trackedQuests);
            }
        }

        private void OnQuestCompleted(Quest quest)
        {
            if (MoveToCompleteQuests(quest.ID) == false)
                Debug.LogError("Failed to complete quest " + quest.QuestData.Name + "! Quest ID: " + quest.ID);
            OnTrackedQuestsChanged?.Invoke(_trackedQuests);
        }

        public bool BeginQuest(string questID)
        {
            if (_completedQuests.ContainsKey(questID))
            {
                Debug.Log("Quest " + _completedQuests[questID].QuestData.Name + " has already been completed!");
                return false;
            }
            if (_activeQuests.ContainsKey(questID))
            {
                Debug.Log("Quest " + _activeQuests[questID].QuestData.Name + " has already been started!");
                return false;
            }
            if (QuestDatabase.Instance.TryGetQuest(questID, out Quest quest))
            {
                quest.Begin();
                return true;
            }
            else
            {
                Debug.LogError("There is no quest with ID " + questID + " in the database!");
                return false;
            }
        }

        private bool MoveToCompleteQuests(string questID)
        {
            if (_activeQuests.ContainsKey(questID))
            {
                Quest quest = _activeQuests[questID];
                if (_completedQuests.TryAdd(questID, quest))
                {
                    _activeQuests.Remove(questID);
                    _currentObjectives.Remove(questID);
                    _trackedQuests.Remove(quest);
                    return true;
                }
                return false;
            }
            return false;
        }

        public bool GetCurrentObjective(string questID, out Objective objective)
        {
            return _currentObjectives.TryGetValue(questID, out objective);
        }
    }
}
