using NaughtyAttributes;
using SunsetSystems.UI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using UnityEditor;
using UnityEngine;

namespace SunsetSystems.Journal
{
    [CreateAssetMenu(fileName = "New Quest", menuName = "Sunset Journal/Quest")]
    [Serializable]
    public class Quest : ScriptableObject, IGameDataProvider<Quest>
    {
        private const string COMPLETE_QUEST = "COMPLETE_QUEST";

        [field: SerializeField, ReadOnly]
        public string ID { get; private set; }
        [field: SerializeField]
        public string ReadableID { get; private set; }
        public string Name;
        public QuestCategory Category;
        [TextArea(10, 15)]
        public string Description;
        public List<Objective> InitialObjectives;
        [field: SerializeField, AllowNesting]
        public List<RewardData> Rewards { get; private set; }

        public Quest Data => this;

        public static event Action<Quest> QuestStarted;
        public static event Action<Quest> QuestCompleted;
        public static event Action<Quest, Objective> ObjectiveCompleted;
        public static event Action<Quest, Objective> ObjectiveFailed;

        private void Awake()
        {
            if (string.IsNullOrWhiteSpace(ID))
            {
                AssignNewID();
            }
            QuestDatabase.Instance.RegisterQuest(this);
        }

        [ContextMenu("Force Validate")]
        private void OnValidate()
        {
            QuestDatabase.Instance.RegisterQuest(this);
        }

        private void Reset()
        {
            AssignNewID();
        }

        private void OnEnable()
        {
            QuestDatabase.Instance.RegisterQuest(this);
        }

        private void OnDestroy()
        {
            QuestDatabase.Instance.UnregisterQuest(this);
        }

        private void AssignNewID()
        {
            ID = System.Guid.NewGuid().ToString();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        public void Begin()
        {
            QuestStarted?.Invoke(this);
            foreach (Objective o in InitialObjectives)
            {
                o.OnObjectiveCompleted += OnObjectiveChanged;
                o.MakeActive();
            }
        }

        //TODO Rework this to sub recursively in quest begin
        private void OnObjectiveChanged(Objective objective)
        {
            Debug.Log($"Completed objective {objective.ID}!");
            ObjectiveCompleted?.Invoke(this, objective);
            objective.OnObjectiveCompleted -= OnObjectiveChanged;
            objective.OnObjectiveInactive -= OnObjectiveDeactivated;
            objective.ObjectivesToCancelOnCompletion.ForEach(o => (o as Objective).MakeInactive());
            if (objective.NextObjectives == null || objective.NextObjectives.Count <= 0)
            {
                Debug.Log($"Completed quest!");
                Complete();
            }
            else
            {
                Debug.Log("Tracking new set of objectives!");
                foreach (UnityEngine.Object o in objective.NextObjectives)
                {
                    Objective ob = o as Objective;
                    ob.OnObjectiveCompleted += OnObjectiveChanged;
                    ob.OnObjectiveInactive += OnObjectiveDeactivated;
                }
            }
        }

        private void OnObjectiveDeactivated(Objective objective)
        {
            objective.OnObjectiveCompleted -= OnObjectiveChanged;
            objective.OnObjectiveInactive -= OnObjectiveDeactivated;
            ObjectiveFailed?.Invoke(this, objective);
        }

        public void Complete()
        {
            Rewards.ForEach(rewardData => (rewardData.Reward as IRewardable).ApplyReward(rewardData.Amount));
            QuestCompleted?.Invoke(this);
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
        [RequireInterface(typeof(IRewardable))]
        public UnityEngine.Object Reward;
    }

    public enum QuestCategory
    {
        Main, Side, Case
    }
}
