using Sirenix.OdinInspector;
using SunsetSystems.Core.Database;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Journal
{
    [CreateAssetMenu(fileName = "New Objective", menuName = "Sunset Journal/Objective")]
    [Serializable]
    public class Objective : SerializedScriptableObject, IDatabaseEntry
    {
        [field: SerializeField, ReadOnly]
        public string DatabaseID { get; private set; }
        [field: SerializeField]
        public string ReadableID { get; private set; }
        public bool IsOptional = false;
        [TextArea(5, 10)]
        public string Description = "";
        public event Action<Objective> OnObjectiveActive;
        public event Action<Objective> OnObjectiveFailed;
        public event Action<Objective> OnObjectiveCanceled;
        public event Action<Objective> OnObjectiveCompleted;

        [ReadOnly]
        public bool IsFirst, IsLast;

        public List<Objective> ObjectivesToCancelOnCompletion;
        public List<Objective> NextObjectives;
        public List<Objective> NextObjectivesOnFailed;
        public List<Objective> ObjectivesToCancelOnFail;

        #region Database Registration
#if UNITY_EDITOR
        private void Awake()
        {
            if (string.IsNullOrWhiteSpace(DatabaseID))
            {
                AssignNewID();
            }
            ObjectiveDatabase.Instance.Register(this);
        }

        [Button("Force Validate")]
        private void OnValidate()
        {
            ObjectiveDatabase.Instance.Register(this);
        }

        private void Reset()
        {
            AssignNewID();
        }

        private void OnDestroy()
        {
            ObjectiveDatabase.Instance.Unregister(this);
        }

        private void AssignNewID()
        {
            DatabaseID = System.Guid.NewGuid().ToString();
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
        #endregion

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
    }
}
