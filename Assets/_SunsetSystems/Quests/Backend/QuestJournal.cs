using Apex;
using CleverCrow.Fluid.UniqueIds;
using SunsetSystems.Data;
using SunsetSystems.LevelManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SunsetSystems.Journal
{
    [RequireComponent(typeof(UniqueId))]
    public class QuestJournal : MonoBehaviour, IResetable, ISaveable, IQuestJournal
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

        public string DataKey => _uniqueId.Id;

        private UniqueId _uniqueId;

        public static event Action<List<Quest>> OnActiveQuestsChanged;

        public void ResetOnGameStart()
        {
            _trackedQuests = new();
            _activeQuests = new();
            _completedQuests = new();
            _currentObjectives = new();
        }

        protected void Awake()
        {
            _uniqueId ??= GetComponent<UniqueId>();
            ISaveable.RegisterSaveable(this);
        }

        [ContextMenu("Foo")]
        public void PrintObjectivesToConsole()
        {
            foreach (string key in _currentObjectives.Keys)
            {
                foreach (string objectiveKey in _currentObjectives[key].Keys)
                {
                    Debug.Log($"Quest: {_activeQuests[key].Name}; Objective ID: {objectiveKey}; Objective Reference: {_currentObjectives[key][objectiveKey]}");
                }
            }
        }

        private void OnEnable()
        {
            Quest.QuestStarted += OnQuestStarted;
            Quest.QuestCompleted += OnQuestCompleted;
            Quest.ObjectiveCompleted += OnQuestObjectiveCompleted;
            Quest.ObjectiveFailed += OnQuestObjectiveFailed;
        }

        private void OnDisable()
        {
            Quest.QuestStarted -= OnQuestStarted;
            Quest.QuestCompleted -= OnQuestCompleted;
            Quest.ObjectiveCompleted -= OnQuestObjectiveCompleted;
            Quest.ObjectiveFailed -= OnQuestObjectiveFailed;
        }

        private void OnDestroy()
        {
            ISaveable.UnregisterSaveable(this);
        }

        private void OnQuestStarted(Quest quest)
        {
            OnActiveQuestsChanged?.Invoke(_trackedQuests);
        }

        private void OnQuestObjectiveCompleted(Quest quest, Objective objective)
        {
            if (objective != null && quest != null)
            {
                if (objective.NextObjectives == null || objective.NextObjectives.Count <= 0)
                    // Do nothing, quest will finish and handle cleanup
                    return;
                if (_currentObjectives.TryGetValue(quest.ID, out Dictionary<string, Objective> questObjectives))
                {
                    questObjectives.Clear();
                    foreach (Objective newObjective in objective.NextObjectives)
                    {
                        questObjectives.Add(newObjective.ReadableID, newObjective);
                    }
                }
                OnActiveQuestsChanged?.Invoke(_trackedQuests);
            }
        }

        private void OnQuestObjectiveFailed(Quest quest, Objective objective)
        {
            if (objective != null && quest != null)
            {
                if (_currentObjectives.TryGetValue(quest.ID, out Dictionary<string, Objective> questObjectives))
                {
                    if (questObjectives.Remove(objective.ReadableID))
                        OnActiveQuestsChanged?.Invoke(_trackedQuests);
                    else
                        Debug.LogWarning($"Trying to fail objective {objective.ReadableID} of quest {quest.Name} but it is not an active objective!");
                }
            }
        }

        private void OnQuestCompleted(Quest quest)
        {
            if (MoveToCompleteQuests(quest.ID) == false)
                Debug.LogError("Failed to complete quest " + quest.Name + "! Quest ID: " + quest.ID);
            OnActiveQuestsChanged?.Invoke(_trackedQuests);
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
                _activeQuests.Add(quest.ID, quest);
                _trackedQuests.Add(quest);
                Dictionary<string, Objective> questObjectives = new();
                quest.InitialObjectives.ForEach(objective => questObjectives.Add(objective.ReadableID, objective));
                _currentObjectives.Add(quest.ID, questObjectives);
                quest.Begin();
                return true;
            }
            else
            {
                Debug.LogError("There is no quest with ID " + questID + " in the database!");
                return false;
            }
        }

        public bool BeginQuestByReadableID(string readableID)
        {
            if (QuestDatabase.Instance.TryGetQuestByReadableID(readableID, out Quest quest))
            {
                return BeginQuest(quest.ID);
            }
            return false;
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

        public bool CompleteObjective(string questID, string objectiveID)
        {
            if (_currentObjectives.TryGetValue(questID, out Dictionary<string, Objective> currentObjectives))
            {
                if (currentObjectives.TryGetValue(objectiveID, out Objective objective))
                {
                    objective.Complete();
                    return true;
                }
            }
            return false;
        }

        public bool FailObjective(string questID, string objectiveID)
        {
            if (_currentObjectives.TryGetValue(questID, out Dictionary<string, Objective> currentObjectives))
            {
                if (currentObjectives.TryGetValue(objectiveID, out Objective objective))
                {
                    objective.MakeInactive();
                    return true;
                }
            }
            return false;
        }

        public object GetSaveData()
        {
            QuestJournalSaveData saveData = new();
            saveData.CurrentObjectives = new();
            foreach (string questKey in _currentObjectives.Keys)
            {
                Dictionary<string, Objective> objectives = new();
                foreach (string objectiveKey in _currentObjectives[questKey].Keys)
                {
                    objectives.Add(objectiveKey, _currentObjectives[questKey][objectiveKey]);
                }
                saveData.CurrentObjectives.Add(questKey, objectives);
            }
            saveData.ActiveQuests = new(_activeQuests);
            saveData.CompletedQuests = new(_completedQuests);
            return saveData;
        }

        public void InjectSaveData(object data)
        {
            QuestJournalSaveData saveData = data as QuestJournalSaveData;
            _activeQuests = new();
            _activeQuests.AddRange(saveData.ActiveQuests);
            _completedQuests = new();
            _completedQuests.AddRange(saveData.CompletedQuests);
            _currentObjectives = new(saveData.CurrentObjectives);
            foreach (string key in _activeQuests.Keys)
            {
                foreach (Objective objective in _currentObjectives[key].Values)
                {
                    _activeQuests[key].ForceSubscribeToObjective(objective);
                }
            }
        }

        private class QuestJournalSaveData : SaveData
        {
            public Dictionary<string, Dictionary<string, Objective>> CurrentObjectives;
            public Dictionary<string, Quest> ActiveQuests, CompletedQuests;
        }
            
    }
}
