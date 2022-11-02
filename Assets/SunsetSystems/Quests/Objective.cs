using System;
using UnityEngine;

namespace SunsetSystems.Journal
{
    public class Objective : ScriptableObject
    {
        public event Action OnActive;
        public event Action<bool> OnCompleted;

        public void StartTracking()
        {
            OnActive?.Invoke();
        }

        public void Complete(bool successful)
        {
            OnCompleted?.Invoke(successful);
        }
    }
}
