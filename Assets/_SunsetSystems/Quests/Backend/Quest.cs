using Sirenix.OdinInspector;
using SunsetSystems.UI.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Journal
{
    [CreateAssetMenu(fileName = "New Quest", menuName = "Sunset Journal/Quest")]
    [Serializable]
    public class Quest : ScriptableObject, IGameDataProvider<Quest>
    {
        [field: SerializeField, ReadOnly]
        public string ID { get; private set; }
        [field: SerializeField]
        public string ReadableID { get; private set; }
        public string Name;
        public QuestCategory Category;
        [TextArea(10, 15)]
        public string Description;
        public List<Objective> InitialObjectives;
        [field: SerializeField]
        public List<RewardData> Rewards { get; private set; }
        [SerializeField, SerializeReference]
        private List<Quest> _startQuestsOnCompletion;
        public Quest Data => this;

        public static event Action<Quest> QuestStarted;
        public static event Action<Quest> QuestCompleted;
        public static event Action<Quest, Objective> ObjectiveCompleted;
        public static event Action<Quest, Objective> ObjectiveFailed;

        private void OnEnable()
        {
#if UNITY_EDITOR
            if (string.IsNullOrWhiteSpace(ID))
            {
                AssignNewID();
            }
            QuestDatabase.Instance?.RegisterQuest(this);
#endif
        }

        [Button("Force Validate")]
        private void OnValidate()
        {
            QuestDatabase.Instance?.RegisterQuest(this);
        }

        private void Reset()
        {
            AssignNewID();
        }

        private void OnDestroy()
        {
#if UNITY_EDITOR
            QuestDatabase.Instance.UnregisterQuest(this);
#endif
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
            Debug.Log($"Completed objective {objective.ReadableID}!");
            ObjectiveCompleted?.Invoke(this, objective);
            objective.OnObjectiveCompleted -= OnObjectiveChanged;
            objective.OnObjectiveInactive -= OnObjectiveDeactivated;
            objective.ObjectivesToCancelOnCompletion.ForEach(o => o.MakeInactive());
            if (objective.NextObjectives == null || objective.NextObjectives.Count <= 0)
            {
                Debug.Log($"Completed quest {Name}!");
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

        public void ForceSubscribeToObjective(Objective objective)
        {
            if (objective == null)
                return;
            objective.OnObjectiveCompleted += OnObjectiveChanged;
            objective.OnObjectiveInactive += OnObjectiveDeactivated;
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
            _startQuestsOnCompletion.ForEach(q => QuestJournal.Instance.BeginQuest(q));
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
