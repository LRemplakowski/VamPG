using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Journal
{
    [Serializable]
    public class Objective
    {
        public event Action OnActive;
        public event Action<bool> OnCompleted;

        public bool IsActive { get; private set; } = false;

        [TextArea(5, 10)]
        public string Description;

        public void StartTracking()
        {
            IsActive = true;
            OnActive?.Invoke();
        }

        public void Complete(bool successful)
        {
            IsActive = false;
            OnCompleted?.Invoke(successful);
        }
    }
}
