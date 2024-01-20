using System;
using Sirenix.OdinInspector;
using UltEvents;
using UnityEngine;
using UnityEngine.Events;

namespace SunsetSystems.Journal
{
    public class ObjectiveEventListener : MonoBehaviour
    {
        [SerializeField]
        private Objective _objective;
        [SerializeField]
        private bool useUltEvents = true;

        [HideIf("@this.useUltEvents == true")]
        public UnityEvent ObjectiveActive, ObjectiveInactive, ObjectiveCompleted;
        [ShowIf("@this.useUltEvents == true")]
        public UltEvent OnObjectiveActive, OnObjectiveInactive, OnObjectiveCompleted;

        private void OnEnable()
        {
            _objective.OnObjectiveActive += ObjectiveActiveHandler;
            _objective.OnObjectiveInactive += ObjectiveInactiveHandler;
            _objective.OnObjectiveCompleted += ObjectiveCompletedHandler;
        }

        private void OnDisable()
        {
            _objective.OnObjectiveActive -= ObjectiveActiveHandler;
            _objective.OnObjectiveInactive -= ObjectiveInactiveHandler;
            _objective.OnObjectiveCompleted -= ObjectiveCompletedHandler;
        }

        private void ObjectiveActiveHandler(Objective obj)
        {
            if (useUltEvents)
                OnObjectiveActive?.InvokeSafe();
            else
                ObjectiveActive?.Invoke();
        }

        private void ObjectiveInactiveHandler(Objective obj)
        {
            if (useUltEvents)
                OnObjectiveInactive?.InvokeSafe();
            else
                ObjectiveInactive?.Invoke();
        }

        private void ObjectiveCompletedHandler(Objective obj)
        {
            if (useUltEvents)
                OnObjectiveCompleted?.InvokeSafe();
            else
                ObjectiveCompleted?.Invoke();
        }
    }
}
