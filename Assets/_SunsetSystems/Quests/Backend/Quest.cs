using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using SunsetSystems.Core.Database;
using SunsetSystems.UI.Utils;
using UnityEngine;

namespace SunsetSystems.Journal
{
    [CreateAssetMenu(fileName = "New Quest", menuName = "Sunset Journal/Quest")]
    [Serializable]
    public class Quest : AbstractDatabaseEntry<Quest>, IUserInfertaceDataProvider<Quest>
    {
        [field: SerializeField, ReadOnly]
        public override string DatabaseID { get; protected set; }
        [field: SerializeField]
        public override string ReadableID { get; protected set; }
        public string Name;
        public QuestCategory Category;
        [TextArea(10, 15)]
        public string Description;
        public List<Objective> InitialObjectives;
        [field: SerializeField]
        public List<RewardData> Rewards { get; private set; }
        [SerializeField, SerializeReference]
        private List<Quest> _startQuestsOnCompletion;
        public Quest UIData => this;

        public static event Action<Quest> QuestStarted;
        public static event Action<Quest> QuestCompleted;
        public static event Action<Quest> QuestFailed;
        public static event Action<Quest, Objective> ObjectiveCompleted;
        public static event Action<Quest, Objective> ObjectiveFailed;

        private HashSet<Objective> _activeObjectives = new();

        public void Begin()
        {
            _activeObjectives = new();
            QuestStarted?.Invoke(this);
            Objective.OnObjectiveCompleted += OnObjectiveCompleted;
            Objective.OnObjectiveFailed += OnObjectiveFailed;
            Objective.OnObjectiveCanceled += OnObjectiveCanceled;
            foreach (Objective o in InitialObjectives)
            {
                o.MakeActive();
                _activeObjectives.Add(o);
            }
        }

        //TODO Rework this to sub recursively in quest begin
        private void OnObjectiveCompleted(Objective objective)
        {
            if (_activeObjectives.Any(ob => ob.DatabaseID == objective.DatabaseID) is false)
                return;
            Debug.Log($"Completed objective {objective.ReadableID}!");
            ObjectiveCompleted?.Invoke(this, objective);
            _activeObjectives.Remove(objective);
            objective.ObjectivesToCancelOnCompletion.ForEach(o => o.Cancel());
            if (objective.NextObjectives == null || objective.NextObjectives.Count <= 0)
            {
                Debug.Log($"Completed quest {Name}!");
                Complete();
            }
            else
            {
                Debug.Log("Tracking new set of objectives!");
                _activeObjectives.AddRange(objective.NextObjectives);
            }
        }

        public void ForceSubscribeToObjective(Objective objective)
        {
            if (objective == null)
                return;
            _activeObjectives ??= new();
            if (_activeObjectives.Count <= 0)
            {
                Objective.OnObjectiveCompleted += OnObjectiveCompleted;
                Objective.OnObjectiveFailed += OnObjectiveFailed;
                Objective.OnObjectiveCanceled += OnObjectiveCanceled;
            }
            _activeObjectives.Add(objective);
        }

        private void OnObjectiveFailed(Objective objective)
        {
            if (_activeObjectives.Any(ob => ob.DatabaseID == objective.DatabaseID) is false)
                return;
            ObjectiveFailed?.Invoke(this, objective);
            _activeObjectives.Remove(objective);
            objective.ObjectivesToCancelOnFail.ForEach(o => o.Cancel());
            if (objective.NextObjectivesOnFailed == null || objective.NextObjectivesOnFailed.Count <= 0)
            {
                Debug.Log($"Failed quest {ReadableID}!");
                Fail();
            }
            else
            {
                _activeObjectives.AddRange(objective.NextObjectivesOnFailed);
            }
        }

        private void OnObjectiveCanceled(Objective objective)
        {
            _activeObjectives.Remove(objective);
        }

        public void Complete()
        {
            Rewards.ForEach(rewardData => rewardData.Reward.ApplyReward(rewardData.Amount));
            QuestCompleted?.Invoke(this);
            _startQuestsOnCompletion.ForEach(q => QuestJournal.Instance.BeginQuest(q));
            Objective.OnObjectiveCompleted -= OnObjectiveCompleted;
            Objective.OnObjectiveFailed -= OnObjectiveFailed;
            Objective.OnObjectiveCanceled -= OnObjectiveCanceled;
        }

        public void Fail()
        {
            QuestFailed?.Invoke(this);
            Objective.OnObjectiveCompleted -= OnObjectiveCompleted;
            Objective.OnObjectiveFailed -= OnObjectiveFailed;
            Objective.OnObjectiveCanceled -= OnObjectiveCanceled;
        }

        protected override void RegisterToDatabase()
        {
            QuestDatabase.Instance.Register(this);
        }

        protected override void UnregisterFromDatabase()
        {
            QuestDatabase.Instance.Unregister(this);
        }
    }

    [Serializable]
    public class ObjectiveList : List<Objective>
    {

    }

    [Serializable]
    public struct QuestData
    {

    }

    [Serializable]
    public struct RewardData
    {
        public int Amount;
        public IRewardable Reward;
    }

    public enum QuestCategory
    {
        Main, Side, Case
    }
}
