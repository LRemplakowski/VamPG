using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using SunsetSystems.Data;
using SunsetSystems.Persistence;
using UnityEngine;

namespace SunsetSystems.Journal
{
    public class QuestJournal : SerializedMonoBehaviour, IResetable, ISaveable
    {
        public static QuestJournal Instance { get; private set; }

        [SerializeField]
        private Dictionary<string, Quest> _activeQuests = new(), _completedQuests = new();
        private Dictionary<string, Dictionary<string, Objective>> _currentObjectives = new();
        [SerializeField]
        private List<Quest> _trackedQuests = new();
        public List<Quest> ActiveQuests => _activeQuests.Values.ToList();
        public List<Quest> MainQuests => _activeQuests.Select(kv => kv.Value).Where(quest => quest.Category.Equals(QuestCategory.Main)).ToList();
        public List<Quest> SideQuests => _activeQuests.Select(kv => kv.Value).Where(quest => quest.Category.Equals(QuestCategory.Side)).ToList();
        public List<Quest> CaseQuests => _activeQuests.Select(kv => kv.Value).Where(quest => quest.Category.Equals(QuestCategory.Case)).ToList();
        public List<Quest> CompletedQuests => _completedQuests.Values.ToList();

        public string DataKey => DataKeyConstants.QUEST_JOURNAL_DATA_KEY;

     

        public static event Action<List<Quest>> OnActiveQuestsChanged;
        public static event Action<HashSet<Objective>> OnObjectiveDataInjected;

        public void ResetOnGameStart()
        {
            _trackedQuests = new();
            _activeQuests = new();
            _completedQuests = new();
            _currentObjectives = new();
        }

        protected void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
           
            ISaveable.RegisterSaveable(this);
        }

        [Button]
        public void PrintObjectivesToConsole()
        {
            foreach (string key in Instance._currentObjectives.Keys)
            {
                foreach (string objectiveKey in Instance._currentObjectives[key].Keys)
                {
                    Debug.Log($"Quest: {Instance._activeQuests[key].Name}; Objective ID: {objectiveKey}; Objective Reference: {Instance._currentObjectives[key][objectiveKey]}");
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

        protected void OnDestroy()
        {
            ISaveable.UnregisterSaveable(this);
        }

        private void OnQuestStarted(Quest quest)
        {
            OnActiveQuestsChanged?.Invoke(_trackedQuests);
        }

        public void OnLevelStarted()
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
                        questObjectives.Add(newObjective.DatabaseID, newObjective);
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

        [Button("Force Start Quest"), EnableIf("@UnityEngine.Application.isPlaying")]
        private void BeginQuestDebug(Quest quest)
        {
            BeginQuest(quest.ID);
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
                quest.InitialObjectives.ForEach(objective => questObjectives.Add(objective.DatabaseID, objective));
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

        public bool TryGetTrackedObjective(string questID, string objectiveID, out Objective objective)
        {
            objective = default;
            if (_currentObjectives.TryGetValue(questID, out Dictionary<string, Objective> questObjectives))
            {
                if (questObjectives.TryGetValue(objectiveID, out objective))
                    return true;
                else
                    Debug.LogWarning($"Trying to get objective {objectiveID} for quest {questID} but that objective is not active!");
            }
            else
            {
                Debug.LogWarning($"Trying to get objectives for quest {questID} but quest is not active!");
            }
            return false;
        }

        public bool TryGetTrackedObjectiveByReadableID(string readableQuestID, string readableObjectiveID, out Objective objective)
        {
            objective = default;
            if (QuestDatabase.Instance.TryGetQuestByReadableID(readableQuestID, out Quest quest))
            {
                if (ObjectiveDatabase.Instance.TryGetEntryByReadableID(readableObjectiveID, out objective))
                {
                    return TryGetTrackedObjective(quest.ID, objective.DatabaseID, out objective);
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
                List<string> objectives = new();
                foreach (string objectiveKey in _currentObjectives[questKey].Keys)
                {
                    objectives.Add(objectiveKey);
                }
                saveData.CurrentObjectives.Add(questKey, objectives);
            }
            saveData.ActiveQuests = _activeQuests.Keys.ToList();
            saveData.CompletedQuests = _completedQuests.Keys.ToList();
            saveData.TrackedQuests = _trackedQuests.Select(quest => quest.ID).ToList();
            return saveData;
        }

        public void InjectSaveData(object data)
        {
            if (data is not QuestJournalSaveData saveData)
                return;
            _activeQuests = new();
            saveData.ActiveQuests.ForEach(questID => { QuestDatabase.Instance.TryGetQuest(questID, out Quest quest); _activeQuests.Add(questID, quest); });
            _completedQuests = new();
            saveData.CompletedQuests.ForEach(questID => { QuestDatabase.Instance.TryGetQuest(questID, out Quest quest); _completedQuests.Add(questID, quest); });
            _currentObjectives = new();
            HashSet<Objective> injectedObjectives = new();
            foreach (string key in saveData.CurrentObjectives.Keys)
            {
                Dictionary<string, Objective> objectives = new();
                saveData.CurrentObjectives[key].ForEach(objectiveID => { ObjectiveDatabase.Instance.TryGetEntry(objectiveID, out Objective objective); objectives.Add(objectiveID, objective); });
                _currentObjectives.Add(key, objectives);
                injectedObjectives.AddRange(objectives.Values);
            }
            _trackedQuests = new();
            saveData.TrackedQuests.ForEach(questID => { QuestDatabase.Instance.TryGetQuest(questID, out Quest quest); _trackedQuests.Add(quest); });
            foreach (string key in _activeQuests.Keys)
            {
                foreach (Objective objective in _currentObjectives[key].Values)
                {
                    _activeQuests[key].ForceSubscribeToObjective(objective);
                }
            }
            OnObjectiveDataInjected?.Invoke(injectedObjectives);
        }

        private class QuestJournalSaveData : SaveData
        {
            public Dictionary<string, List<string>> CurrentObjectives;
            public List<string> ActiveQuests, CompletedQuests, TrackedQuests;
        }
            
    }
}
