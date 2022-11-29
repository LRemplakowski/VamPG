using SunsetSystems.Dialogue;
using UnityEngine;
using Yarn.Unity;

namespace SunsetSystems.Entities.Interactable
{
    public class DialogueEntity : InteractableEntity
    {
        [SerializeField]
        private YarnProject _dialogueProject;
        [field: SerializeField]
        public string EntryNode { get; set; }

        protected override void HandleInteraction()
        {
            DialogueManager.Instance.StartDialogue(EntryNode, _dialogueProject);
        }
    }
}
