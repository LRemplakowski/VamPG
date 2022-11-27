using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Journal
{
    [Serializable]
    [CreateAssetMenu(fileName = "New Objective", menuName = "Sunset Journal/Objective")]
    public class Objective : ScriptableObject
    {
        [ReadOnly]
        public string ID = "";
        [TextArea(5, 10)]
        public string Description = "";
        public event Action<Objective> OnObjectiveActive;
        public event Action<Objective> OnObjectiveInactive;
        public event Action<Objective> OnObjectiveCompleted;

        [ReadOnly]
        public bool IsFirst, IsLast;

        [RequireInterface(typeof(Objective))]
        public List<UnityEngine.Object> ObjectivesToCancelOnCompletion;
        [RequireInterface(typeof(Objective))]
        public List<UnityEngine.Object> NextObjectives;

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(ID))
                AssignNewID();
        }

        private void Reset()
        {
            AssignNewID();
        }

        private void AssignNewID()
        {
            ID = System.Guid.NewGuid().ToString();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        public void MakeActive()
        {
            OnObjectiveActive?.Invoke(this);
        }

        public void MakeInactive()
        {
            OnObjectiveInactive?.Invoke(this);
        }

        public void Complete()
        {
            MakeInactive();
            OnObjectiveCompleted?.Invoke(this);
            ObjectivesToCancelOnCompletion.ForEach(o => (o as Objective).MakeInactive());
            NextObjectives.ForEach(o => (o as Objective).MakeActive());
        }
    }
}
