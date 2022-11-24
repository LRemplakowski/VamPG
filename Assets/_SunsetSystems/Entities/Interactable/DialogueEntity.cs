using UnityEngine;

namespace SunsetSystems.Entities.Interactable
{
    public class DialogueEntity : InteractableEntity
    {
        [SerializeField]
        private TextAsset entityDialogue;

        protected override void HandleInteraction()
        {

        }
    }
}
