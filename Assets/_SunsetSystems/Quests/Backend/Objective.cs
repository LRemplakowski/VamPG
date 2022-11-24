using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Journal
{
    [Serializable]
    public class Objective
    {
        public string ID;
        [TextArea(5, 10)]
        public string Description;
        public event Action<Objective> OnObjectiveActive;
        public event Action<Objective> OnObjectiveInactive;
        public event Action<Objective> OnObjectiveCompleted;

        [ReadOnly]
        public bool IsFirst, IsLast;

        public Objective NextObjective;

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
            if (NextObjective != null)
                NextObjective.MakeActive();
        }
    }
}
