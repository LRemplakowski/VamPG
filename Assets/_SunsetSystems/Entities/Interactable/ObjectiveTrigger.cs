using UnityEngine;
using SunsetSystems.Journal;
using Sirenix.OdinInspector;
using SunsetSystems.Entities.Characters.Actions;

namespace SunsetSystems.Entities.Interactable
{
    public class ObjectiveTrigger : SerializedMonoBehaviour, IObjectiveTrigger, IInteractionHandler
    {
        [SerializeReference, Required]
        private Quest _associatedQuest;
        [SerializeField]
        private Objective _objective;
        private bool ObjectiveActive = false;

        protected void Start()
        {
            if (_objective != null)
            {
                _objective.OnObjectiveActive -= OnObjectiveActive;
                _objective.OnObjectiveFailed -= OnObjectiveInactive;
                _objective.OnObjectiveActive += OnObjectiveActive;
                _objective.OnObjectiveFailed += OnObjectiveInactive;
            }
        }

        protected void OnDestroy()
        {
            if (_objective != null)
            {
                _objective.OnObjectiveActive -= OnObjectiveActive;
                _objective.OnObjectiveFailed -= OnObjectiveInactive;
            }
        }

        private void OnObjectiveActive(Objective objective)
        {
            ObjectiveActive = true;
        }

        private void OnObjectiveInactive(Objective objective)
        {
            ObjectiveActive = false;
        }

        public bool CheckCompletion(Objective objective)
        {
            return ObjectiveActive;
        }

        public bool HandleInteraction(IActionPerformer interactee)
        {
            if (CheckCompletion(_objective))
            {
                Debug.Log($"Completed objective {_objective}!");
                _objective.Complete();
                return true;
            }
            else
            {
                Debug.Log($"Objective {_objective} is not active!");
                return false;
            }
        }
    }
}
