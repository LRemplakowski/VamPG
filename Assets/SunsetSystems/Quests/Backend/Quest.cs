using NaughtyAttributes;
using SunsetSystems.UI.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Journal
{
    [CreateAssetMenu(fileName = "New Quest", menuName = "Sunset Journal/Quest")]
    public class Quest : ScriptableObject, IGameDataProvider<Quest>
    {
        [field: SerializeField, ReadOnly]
        public string ID { get; private set; }
        [field: SerializeField, AllowNesting]
        public QuestData Info { get; private set; }
        [field: SerializeField, AllowNesting]
        public List<RewardData> Rewards { get; private set; }

        public Quest Data => this;

        public static event Action<Quest> QuestStarted;
        public static event Action<Quest> QuestCompleted;
        public static event Action<Quest, Objective> ObjectiveChanged;

        public void LinkAndInitializeObjectives()
        {
            List<Objective> objectives = Info.Objectives;
            if (objectives == null || objectives.Count <= 0)
                return;
            for (int i = objectives.Count - 1; i > 0; i--)
            {
                Objective next = objectives[i];
                objectives[i - 1].NextObjective = next;
                next.IsLast = false;
                next.IsFirst = false;
                if (string.IsNullOrEmpty(next.ID))
                    next.ID = $"Objective {i}";
            }
            Objective last = objectives[^1];
            last.IsLast = true;
            last.IsFirst = false;
            Objective first = objectives[0];
            if (string.IsNullOrEmpty(first.ID))
                first.ID = "Objective 0";
            if (last.Equals(first))
            {
                last.IsFirst = true;
            }
            else
            {
                first.IsFirst = true;
                first.IsLast = false;
            }
        }

        private void OnValidate()
        {
            LinkAndInitializeObjectives();
        }

        private void OnEnable()
        {
            if (string.IsNullOrEmpty(ID))
                ID = Guid.NewGuid().ToString();
            if (QuestDatabase.Instance.IsRegistered(this) == false)
                QuestDatabase.Instance.RegisterQuest(this);
        }

        public void Begin()
        {
            QuestStarted?.Invoke(this);
            Info.FirstObjective.OnObjectiveCompleted += OnObjectiveChanged;
            Info.FirstObjective.MakeActive();
        }

        private void OnObjectiveChanged(Objective objective)
        {
            ObjectiveChanged?.Invoke(this, objective);
            if (objective == null)
                return;
            objective.OnObjectiveCompleted -= OnObjectiveChanged;
            if (objective.NextObjective == null)
            {
                Complete();
            }
            else
            {
                objective.NextObjective.OnObjectiveCompleted += OnObjectiveChanged;
            }
        }

        public void Complete()
        {
            Rewards.ForEach(rewardData => (rewardData.Reward as IRewardable).ApplyReward(rewardData.Amount));
            QuestCompleted?.Invoke(this);
        }
    }

    [Serializable]
    public struct QuestData
    {
        public string Name;
        public QuestCategory Category;
        [TextArea(10, 15)]
        public string Description;
        [ReorderableList, AllowNesting]
        public List<Objective> Objectives;
        public Objective FirstObjective => Objectives?[0];
        public Objective LastObjective => Objectives?[^1];
    }

    [Serializable]
    public struct RewardData
    {
        public int Amount;
        [RequireInterface(typeof(IRewardable))]
        public UnityEngine.Object Reward;
    }

    public enum QuestCategory
    {
        Main, Side, Case
    }
}
