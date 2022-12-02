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

        public void MakeActive()
        {
            Debug.Log($"Begun tracking objective {ID}!");
            OnObjectiveActive?.Invoke(this);
        }

        public void MakeInactive()
        {
            Debug.Log($"Failed objective {ID}!");
            OnObjectiveInactive?.Invoke(this);
        }

        public void Complete()
        {
            Debug.Log($"Completed objective {ID}!");
            OnObjectiveCompleted?.Invoke(this);
            ObjectivesToCancelOnCompletion.ForEach(o => (o as Objective).MakeInactive());
            NextObjectives.ForEach(o => (o as Objective).MakeActive());
        }
    }
}
