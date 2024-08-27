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
                Objective.OnObjectiveActive -= OnObjectiveActive;
                Objective.OnObjectiveFailed -= OnObjectiveInactive;
                Objective.OnObjectiveActive += OnObjectiveActive;
                Objective.OnObjectiveFailed += OnObjectiveInactive;
            }
        }

        protected void OnDestroy()
        {
            if (_objective != null)
            {
                Objective.OnObjectiveActive -= OnObjectiveActive;
                Objective.OnObjectiveFailed -= OnObjectiveInactive;
            }
        }

        private void OnObjectiveActive(Objective objective)
        {
            if (objective.DatabaseID == _objective.DatabaseID)
                ObjectiveActive = true;
        }

        private void OnObjectiveInactive(Objective objective)
        {
            if (objective.DatabaseID == _objective.DatabaseID)
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
