using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
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

        public void Begin()
        {
            QuestStarted?.Invoke(this);
            foreach (Objective o in InitialObjectives)
            {
                o.OnObjectiveCompleted += OnObjectiveCompleted;
                o.OnObjectiveFailed += OnObjectiveFailed;
                o.OnObjectiveCanceled += OnObjectiveCanceled;
                o.MakeActive();
            }
        }

        //TODO Rework this to sub recursively in quest begin
        private void OnObjectiveCompleted(Objective objective)
        {
            Debug.Log($"Completed objective {objective.ReadableID}!");
            ObjectiveCompleted?.Invoke(this, objective);
            objective.OnObjectiveCompleted -= OnObjectiveCompleted;
            objective.OnObjectiveFailed -= OnObjectiveFailed;
            objective.ObjectivesToCancelOnCompletion.ForEach(o => o.Cancel());
            if (objective.NextObjectives == null || objective.NextObjectives.Count <= 0)
            {
                Debug.Log($"Completed quest {Name}!");
                Complete();
            }
            else
            {
                Debug.Log("Tracking new set of objectives!");
                foreach (Objective newObjective in objective.NextObjectives)
                {
                    newObjective.OnObjectiveCompleted += OnObjectiveCompleted;
                    newObjective.OnObjectiveFailed += OnObjectiveFailed;
                    newObjective.OnObjectiveCanceled += OnObjectiveCanceled;
                }
            }
        }

        public void ForceSubscribeToObjective(Objective objective)
        {
            if (objective == null)
                return;
            objective.OnObjectiveCompleted += OnObjectiveCompleted;
            objective.OnObjectiveFailed += OnObjectiveFailed;
            objective.OnObjectiveCanceled += OnObjectiveCanceled;
        }

        private void OnObjectiveFailed(Objective objective)
        {
            ObjectiveFailed?.Invoke(this, objective);
            objective.OnObjectiveCompleted -= OnObjectiveCompleted;
            objective.OnObjectiveFailed -= OnObjectiveFailed;
            objective.OnObjectiveCanceled -= OnObjectiveCanceled;
            objective.ObjectivesToCancelOnFail.ForEach(o => o.Cancel());
            if (objective.NextObjectivesOnFailed == null || objective.NextObjectivesOnFailed.Count <= 0)
            {
                Debug.Log($"Failed quest {ReadableID}!");
                Fail();
            }
            else
            {
                foreach (Objective newObjective in objective.NextObjectivesOnFailed)
                {
                    newObjective.OnObjectiveCompleted += OnObjectiveCompleted;
                    newObjective.OnObjectiveFailed += OnObjectiveFailed;
                    newObjective.OnObjectiveCanceled += OnObjectiveCanceled;
                }
            }
        }

        private void OnObjectiveCanceled(Objective objective)
        {
            objective.OnObjectiveCompleted -= OnObjectiveCompleted;
            objective.OnObjectiveFailed -= OnObjectiveFailed;
            objective.OnObjectiveCanceled -= OnObjectiveCanceled;
        }

        public void Complete()
        {
            Rewards.ForEach(rewardData => rewardData.Reward.ApplyReward(rewardData.Amount));
            QuestCompleted?.Invoke(this);
            _startQuestsOnCompletion.ForEach(q => QuestJournal.Instance.BeginQuest(q));
        }

        public void Fail()
        {
            QuestFailed?.Invoke(this);
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
