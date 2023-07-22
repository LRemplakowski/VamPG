using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Journal
{
    [CreateAssetMenu(fileName = "New Objective", menuName = "Sunset Journal/Objective")]
    [Serializable]
    public class Objective : ScriptableObject
    {
        [field: SerializeField, ReadOnly]
        public string DatabaseID { get; private set; }
        public string ReadableID = "";
        [TextArea(5, 10)]
        public string Description = "";
        public event Action<Objective> OnObjectiveActive;
        public event Action<Objective> OnObjectiveInactive;
        public event Action<Objective> OnObjectiveCompleted;

        [ReadOnly]
        public bool IsFirst, IsLast;

        public List<Objective> ObjectivesToCancelOnCompletion;
        public List<Objective> NextObjectives;

        #region Database Registration

        private void OnEnable()
        {
#if UNITY_EDITOR
            if (string.IsNullOrWhiteSpace(DatabaseID))
            {
                AssignNewID();
            }
            ObjectiveDatabase.Instance?.Register(this);
#endif
        }

        [Button("Force Validate")]
        private void OnValidate()
        {
            ObjectiveDatabase.Instance?.Register(this);
        }
#if UNITY_EDITOR
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

        public void MakeInactive()
        {
            Debug.Log($"Failed objective {ReadableID}!");
            OnObjectiveInactive?.Invoke(this);
        }

        public void Complete()
        {
            Debug.Log($"Completed objective {ReadableID}!");
            OnObjectiveCompleted?.Invoke(this);
            ObjectivesToCancelOnCompletion.ForEach(o => (o as Objective).MakeInactive());
            NextObjectives.ForEach(o => (o as Objective).MakeActive());
        }
    }
}
