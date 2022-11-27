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
        [SerializeField]
        private bool _fireOnce;
        private bool _fired;

        protected override void HandleInteraction()
        {
            if (_fireOnce && _fired)
                return;
            DialogueManager.StartDialogue(EntryNode, _dialogueProject);
            _fired = true;
        }
    }
}
