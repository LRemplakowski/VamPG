using System.Collections;
using System.Collections.Generic;
using SunsetSystems.Dialogue;
using UnityEngine;

namespace SunsetSystems.Entities.Interactable
{
    public class DialogueEntity : InteractableEntity
    {
        [SerializeField]
        private TextAsset entityDialogue;

        public override void Interact()
        {
            base.Interact();
        }
    }
}
