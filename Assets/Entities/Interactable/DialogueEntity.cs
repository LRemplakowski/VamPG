using System.Collections;
using System.Collections.Generic;
using SunsetSystems.Dialogue;
using SunsetSystems.Management;
using UnityEngine;

namespace Entities.Interactable
{
    public class DialogueEntity : InteractableEntity
    {
        [SerializeField]
        private TextAsset entityDialogue;

        public override void Interact()
        {
            References.Get<DialogueManager>().StartDialogue(entityDialogue);
            base.Interact();
        }
    } 
}
