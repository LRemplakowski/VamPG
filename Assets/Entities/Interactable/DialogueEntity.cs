using System.Collections;
using System.Collections.Generic;
using Systems.Management;
using UnityEngine;

namespace Entities.Interactable
{
    public class DialogueEntity : InteractableEntity
    {
        [SerializeField]
        private TextAsset entityDialogue;

        public override void Interact()
        {
            ReferenceManager.GetManager<DialogueManager>().StartDialogue(entityDialogue);
            base.Interact();
        }
    } 
}
