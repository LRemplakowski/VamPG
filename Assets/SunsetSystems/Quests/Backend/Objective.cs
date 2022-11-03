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
        public event Action OnBeginTracking;
        public event Action OnStopTracking;
        public event Action OnCompleted;

        [ReadOnly]
        public bool IsFirst, IsLast;

        public Objective NextObjective;

        public void StartTracking()
        {
            OnBeginTracking?.Invoke();
        }

        public void StopTracking()
        {
            OnStopTracking?.Invoke();
        }

        public void Complete()
        {
            OnCompleted?.Invoke();
            if (NextObjective != null)
                NextObjective.StartTracking();
        }
    }
}
