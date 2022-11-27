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
        private Dictionary<string, Dictionary<string, Objective>> _currentObjectives = new();
        [SerializeField]
        private List<Quest> _trackedQuests = new();

        public List<Quest> ActiveQuests => _activeQuests.Values.ToList();
        public List<Quest> MainQuests => _activeQuests.Select(kv => kv.Value).Where(quest => quest.Category.Equals(QuestCategory.Main)).ToList();
        public List<Quest> SideQuests => _activeQuests.Select(kv => kv.Value).Where(quest => quest.Category.Equals(QuestCategory.Side)).ToList();
        public List<Quest> CaseQuests => _activeQuests.Select(kv => kv.Value).Where(quest => quest.Category.Equals(QuestCategory.Case)).ToList();
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
            Dictionary<string, Objective> questObjectives = new();
            quest.InitialObjectives.ForEach(o => questObjectives.Add(o.ID, o));
            _currentObjectives.Add(quest.ID, questObjectives);
            OnTrackedQuestsChanged?.Invoke(_trackedQuests);
        }

        private void OnQuestObjectiveChanged(Quest quest, Objective objective)
        {
            if (objective != null && quest != null)
            {
                if (objective.NextObjectives == null || objective.NextObjectives.Count <= 0)
                    // Do nothing, quest will finish and handle cleanup
                    return;
                if (_currentObjectives.TryGetValue(quest.ID, out Dictionary<string, Objective> currentObjectives))
                {
                    if (currentObjectives.ContainsKey(objective.ID))
                    {
                        currentObjectives.Clear();
                        foreach (Objective newObjective in objective.NextObjectives)
                        {
                            currentObjectives.Add(newObjective.ID, newObjective);
                        }
                    }
                }
                else
                {
                    Dictionary<string, Objective> questObjectives = new();
                    quest.InitialObjectives.ForEach(o => questObjectives.Add(o.ID, o));
                    _currentObjectives.Add(quest.ID, questObjectives);
                }
                OnTrackedQuestsChanged?.Invoke(_trackedQuests);
            }
        }

        private void OnQuestCompleted(Quest quest)
        {
            if (MoveToCompleteQuests(quest.ID) == false)
                Debug.LogError("Failed to complete quest " + quest.Name + "! Quest ID: " + quest.ID);
            OnTrackedQuestsChanged?.Invoke(_trackedQuests);
        }

        public bool IsQuestCompleted(string questID)
        {
            return _completedQuests.ContainsKey(questID);
        }

        public bool BeginQuest(string questID)
        {
            if (_completedQuests.ContainsKey(questID))
            {
                Debug.Log("Quest " + _completedQuests[questID].Name + " has already been completed!");
                return false;
            }
            if (_activeQuests.ContainsKey(questID))
            {
                Debug.Log("Quest " + _activeQuests[questID].Name + " has already been started!");
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

        public bool GetCurrentObjectives(string questID, out List<Objective> objectives)
        {
            objectives = null;
            if (_currentObjectives.TryGetValue(questID, out Dictionary<string, Objective> currentObjectives))
            {
                objectives = currentObjectives.Values.ToList();
                return true;
            }
            return false;
        }
    }
}
