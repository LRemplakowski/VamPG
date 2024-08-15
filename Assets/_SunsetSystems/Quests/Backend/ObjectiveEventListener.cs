using System.Collections.Generic;
using System.Linq;
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
        public UnityEvent ObjectiveActive, ObjectiveInactive, ObjectiveCompleted, ObjectiveActiveAfterSceneLoad;
        [ShowIf("@this.useUltEvents == true")]
        public UltEvent OnObjectiveActive, OnObjectiveFailed, OnObjectiveCompleted, OnObjectiveActiveAfterSceneLoad;

        private void OnEnable()
        {
            Objective.OnObjectiveActive += ObjectiveActiveHandler;
            Objective.OnObjectiveFailed += ObjectiveFailedHandler;
            Objective.OnObjectiveCompleted += ObjectiveCompletedHandler;
            QuestJournal.OnObjectiveDataInjected += ObjectiveDataInjectedHandler;
        }

        private void OnDisable()
        {
            Objective.OnObjectiveActive -= ObjectiveActiveHandler;
            Objective.OnObjectiveFailed -= ObjectiveFailedHandler;
            Objective.OnObjectiveCompleted -= ObjectiveCompletedHandler;
            QuestJournal.OnObjectiveDataInjected -= ObjectiveDataInjectedHandler;
        }

        private void ObjectiveActiveHandler(Objective obj)
        {
            if (obj.DatabaseID == _objective.DatabaseID)
            {
                if (useUltEvents)
                    OnObjectiveActive?.InvokeSafe();
                else
                    ObjectiveActive?.Invoke();
            }
        }

        private void ObjectiveFailedHandler(Objective obj)
        {
            if (obj.DatabaseID == _objective.DatabaseID)
            {
                if (useUltEvents)
                    OnObjectiveFailed?.InvokeSafe();
                else
                    ObjectiveInactive?.Invoke();
            }
        }

        private void ObjectiveCompletedHandler(Objective obj)
        {
            if (obj.DatabaseID == _objective.DatabaseID)
            {
                if (useUltEvents)
                    OnObjectiveCompleted?.InvokeSafe();
                else
                    ObjectiveCompleted?.Invoke();
            }
        }

        private void ObjectiveDataInjectedHandler(HashSet<Objective> objectives)
        {
            if (objectives.Any(ob => ob.DatabaseID == _objective.DatabaseID))
            {
                if (useUltEvents)
                    OnObjectiveActiveAfterSceneLoad?.InvokeSafe();
                else
                    ObjectiveActiveAfterSceneLoad?.Invoke();
            }
        }
    }
}
