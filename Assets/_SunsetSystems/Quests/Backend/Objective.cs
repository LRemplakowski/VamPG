using Sirenix.OdinInspector;
using SunsetSystems.Core.Database;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Journal
{
    [CreateAssetMenu(fileName = "New Objective", menuName = "Sunset Journal/Objective")]
    [Serializable]
    public class Objective : AbstractDatabaseEntry<Objective>
    {
        [field: SerializeField, ReadOnly]
        public override string DatabaseID { get; protected set; }
        [field: SerializeField]
        public override string ReadableID { get; protected set; }
        public bool IsOptional = false;
        [TextArea(5, 10)]
        public string Description = "";
        public static event Action<Objective> OnObjectiveActive;
        public static event Action<Objective> OnObjectiveFailed;
        public static event Action<Objective> OnObjectiveCanceled;
        public static event Action<Objective> OnObjectiveCompleted;

        [ReadOnly]
        public bool IsFirst, IsLast;

        public List<Objective> ObjectivesToCancelOnCompletion;
        public List<Objective> NextObjectives;
        public List<Objective> NextObjectivesOnFailed;
        public List<Objective> ObjectivesToCancelOnFail;

        public void MakeActive()
        {
            Debug.Log($"Begun tracking objective {ReadableID}!");
            OnObjectiveActive?.Invoke(this);
        }

        public void Cancel()
        {
            Debug.Log($"Canceled objective {ReadableID}!");
            OnObjectiveCanceled?.Invoke(this);
        }

        public void Fail()
        {
            Debug.Log($"Failed objective {ReadableID}!");
            OnObjectiveFailed?.Invoke(this);
            ObjectivesToCancelOnFail.ForEach(o => o.Cancel());
            NextObjectivesOnFailed.ForEach(o => o.MakeActive());
        }

        [Button]
        public void Complete()
        {
            Debug.Log($"Completed objective {ReadableID}!");
            OnObjectiveCompleted?.Invoke(this);
            ObjectivesToCancelOnCompletion.ForEach(o => o.Cancel());
            NextObjectives.ForEach(o => o.MakeActive());
        }

        protected override void RegisterToDatabase()
        {
            ObjectiveDatabase.Instance.Register(this);
        }

        protected override void UnregisterFromDatabase()
        {
            ObjectiveDatabase.Instance.Unregister(this);
        }
    }
}
