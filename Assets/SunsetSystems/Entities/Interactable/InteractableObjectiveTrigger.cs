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
        private Objective Objective => _associatedQuest.QuestData.Objectives.Find(o => o.ID.Equals(_objectiveID));
        private bool ObjectiveActive = false;

        private void Awake()
        {
            Objective.OnObjectiveActive += OnObjectiveActive;
            Objective.OnObjectiveInactive += OnObjectiveInactive;
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

        public override void Interact()
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
            base.Interact();
        }

        public string[] Objectives()
        {
            if (_associatedQuest == null)
                return new string[0];
            return _associatedQuest.QuestData.Objectives.Select(o => o.ID).ToArray();
        }
    }
}
