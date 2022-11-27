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
    public class Quest : ScriptableObject, IGameDataProvider<Quest>
    {
        private const string COMPLETE_QUEST = "COMPLETE_QUEST";

        [field: SerializeField, ReadOnly]
        public string ID { get; private set; }
        public string Name;
        public QuestCategory Category;
        [TextArea(10, 15)]
        public string Description;
        public List<Objective> InitialObjectives;
#if UNITY_EDITOR
        //[Button]
        //private void AddObjective()
        //{
        //    Objective newObjective = CreateInstance<Objective>();
        //    AssetDatabase.AddObjectToAsset(newObjective, this);
        //    AssetDatabase.SaveAssets();
        //    Objectives.Add(newObjective.ID, newObjective);
        //    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(newObjective));
        //    _objectiveAssetPaths.Add(AssetDatabase.GetAssetPath(newObjective));
        //}
#endif
        [field: SerializeField, AllowNesting]
        public List<RewardData> Rewards { get; private set; }

        public Quest Data => this;

        public static event Action<Quest> QuestStarted;
        public static event Action<Quest> QuestCompleted;
        public static event Action<Quest, Objective> ObjectiveChanged;

        public void LinkAndInitializeObjectives()
        {
            //List<Objective> objectives = Objectives.ToList();
            //if (objectives == null || objectives.Count <= 0)
            //    return;
            //foreach (Objective objective in objectives)
            //{
            //    if (string.IsNullOrWhiteSpace(objective.NextObjectiveID) == false)
            //    {
            //        if (objective.NextObjectiveID.Equals(COMPLETE_QUEST))
            //        {
            //            objective.NextObjective = null;
            //        }
            //        else
            //        {
            //            objective.NextObjective = objectives.Find(o => o.Name.Equals(objective.NextObjectiveID));
            //        }
            //    }
            //}
            //Objective last = objectives[^1];
            //last.IsLast = true;
            //last.IsFirst = false;
            //Objective first = objectives[0];
            //if (string.IsNullOrEmpty(first.Name))
            //    first.Name = "Objective 0";
            //if (last.Equals(first))
            //{
            //    last.IsFirst = true;
            //}
            //else
            //{
            //    first.IsFirst = true;
            //    first.IsLast = false;
            //}
        }

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(ID))
            {
                AssignNewID();
            }
            //if (EditorUtility.IsDirty(this))
            //{
            //    Objectives.Clear();
            //    AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(this)).ToList().ForEach(a => { if (a is Objective objective) Objectives.Add(objective); });
            //}
            //LinkAndInitializeObjectives();
        }

        private void Reset()
        {
            AssignNewID();
        }

        private void OnEnable()
        {
            if (string.IsNullOrWhiteSpace(ID) == false && QuestDatabase.Instance.IsRegistered(this) == false)
                QuestDatabase.Instance.RegisterQuest(this);
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

        private void OnObjectiveChanged(Objective objective)
        {
            ObjectiveChanged?.Invoke(this, objective);
            if (objective == null)
                return;
            objective.OnObjectiveCompleted -= OnObjectiveChanged;
            objective.ObjectivesToCancelOnCompletion.ForEach(o => (o as Objective).OnObjectiveCompleted -= OnObjectiveChanged);
            if (objective.NextObjectives == null || objective.NextObjectives.Count <= 0)
            {
                Complete();
            }
            else
            {
                objective.NextObjectives.ForEach(o => (o as Objective).OnObjectiveCompleted += OnObjectiveChanged);
            }
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
