using System;
using UnityEngine;
using UnityEngine.Events;

namespace SunsetSystems.Journal
{
    public class ObjectiveEventListener : MonoBehaviour
    {
        [SerializeField]
        private Objective _objective;

        public UnityEvent ObjectiveActive, ObjectiveInactive, ObjectiveCompleted;

        private void OnEnable()
        {
            _objective.OnObjectiveActive += OnObjectiveActive;
            _objective.OnObjectiveInactive += OnObjectiveInactive;
            _objective.OnObjectiveCompleted += OnObjectiveCompleted;
        }

        private void OnDisable()
        {
            _objective.OnObjectiveActive -= OnObjectiveActive;
            _objective.OnObjectiveInactive -= OnObjectiveInactive;
            _objective.OnObjectiveCompleted -= OnObjectiveCompleted;
        }

        private void OnObjectiveActive(Objective obj)
        {
            ObjectiveActive?.Invoke();
        }

        private void OnObjectiveInactive(Objective obj)
        {
            ObjectiveInactive?.Invoke();
        }

        private void OnObjectiveCompleted(Objective obj)
        {
            ObjectiveCompleted?.Invoke();
        }
    }
}
