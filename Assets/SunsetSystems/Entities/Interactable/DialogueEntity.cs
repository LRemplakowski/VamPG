using System.Collections;
using System.Collections.Generic;
using SunsetSystems.Dialogue;
using UnityEngine;

namespace Entities.Interactable
{
    public class DialogueEntity : InteractableEntity
    {
        [SerializeField]
        private TextAsset entityDialogue;

        public override void Interact()
        {
            DialogueManager.Instance.StartDialogue(entityDialogue);
            base.Interact();
        }
    }
}
