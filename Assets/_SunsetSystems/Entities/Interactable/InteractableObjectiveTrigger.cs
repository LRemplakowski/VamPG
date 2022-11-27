using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SunsetSystems.Journal;
using NaughtyAttributes;
using System.Linq;

namespace SunsetSystems.Entities.Interactable
{
    public class InteractableObjectiveTrigger : InteractableEntity, IObjectiveTrigger
    {
        [SerializeReference, Required]
        private Quest _associatedQuest;
        [SerializeField, Dropdown("Objectives")]
        private string _objectiveID;
        private Objective Objective => _associatedQuest.Info.Objectives.Find(o => o.ID.Equals(_objectiveID));
        private bool ObjectiveActive = false;

        protected override void Start()
        {
            base.Start();
            if (Objective != null)
            {
                Objective.OnObjectiveActive -= OnObjectiveActive;
                Objective.OnObjectiveInactive -= OnObjectiveInactive;
                Objective.OnObjectiveActive += OnObjectiveActive;
                Objective.OnObjectiveInactive += OnObjectiveInactive;
            }
        }

        private void OnDestroy()
        {
            if (Objective != null)
            {
                Objective.OnObjectiveActive -= OnObjectiveActive;
                Objective.OnObjectiveInactive -= OnObjectiveInactive;
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            if (string.IsNullOrEmpty(_objectiveID))
            {
                Debug.LogWarning($"Null or empty objective ID in GameObject {gameObject.name}! Resetting!");
                if (_associatedQuest)
                    _objectiveID = _associatedQuest.Info.FirstObjective?.ID;
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

        protected override void HandleInteraction()
        {
            if (CheckCompletion(Objective))
            {
                Debug.Log($"Completed objective {_objectiveID}!");
                Objective.Complete();
            }
            else
            {
                Debug.Log($"Objective {_objectiveID} is not active!");
            }
        }

        public string[] Objectives()
        {
            if (_associatedQuest == null)
                return new string[0];
            return _associatedQuest.Info.Objectives.Select(o => o.ID).ToArray();
        }
    }
}
