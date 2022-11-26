using SunsetSystems.Dialogue;
using UnityEngine;
using Yarn.Unity;

namespace SunsetSystems.Entities.Interactable
{
    public class DialogueEntity : InteractableEntity
    {
        [SerializeField]
        private YarnProject _dialogueProject;
        [SerializeField]
        private string _entryNode;
        [SerializeField]
        private bool _fireOnce;
        private bool _fired;

        protected override void HandleInteraction()
        {
            if (_fireOnce && _fired)
                return;
            DialogueManager.StartDialogue(_entryNode, _dialogueProject);
            _fired = true;
        }
    }
}
