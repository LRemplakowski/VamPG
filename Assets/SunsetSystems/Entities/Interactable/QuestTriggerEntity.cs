using SunsetSystems.Journal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Entities.Interactable
{
    public class QuestTriggerEntity : InteractableEntity, IQuestTrigger
    {
        [SerializeReference]
        private Quest _myQuest;
        public Quest MyQuest => _myQuest;

        public override void Interact()
        {
            (this as IQuestTrigger).TriggerQuest();
            base.Interact();
        }
    }
}
