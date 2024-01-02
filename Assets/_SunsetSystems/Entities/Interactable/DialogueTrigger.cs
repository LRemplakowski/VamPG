using Sirenix.OdinInspector;
using SunsetSystems.Dialogue;
using SunsetSystems.Dialogue.Interfaces;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;
using SunsetSystems.Entities.Characters.Actions;

namespace SunsetSystems.Entities.Interactable
{
    public class DialogueTrigger : SerializedMonoBehaviour, IInteractionHandler, IDialogueSource
    {
        [SerializeField]
        private bool _fadeOutBeforeDialogue;
        [InfoBox("XYZ represent fade out timings.\nX - Fading out time\nY - Time screen stays faded out\nZ - Fading in time")]
        [Space]
        [SerializeField, ShowIf("_fadeOutBeforeDialogue")]
        private Vector3 _fadeTimes = Vector3.one;
        [SerializeField, ShowIf("_fadeOutBeforeDialogue")]
        private UnityEvent _onAfterFadeout;
        [SerializeField]
        private YarnProject _dialogueProject;
        [field: SerializeField]
        public string EntryNode { get; set; }

        public void StartDialogue(string dialogueID)
        {
            if (_fadeOutBeforeDialogue)
            {
                _onAfterFadeout?.Invoke();
            }
            DialogueManager.Instance.StartDialogue(dialogueID, _dialogueProject);
        }

        public bool HandleInteraction(IActionPerformer interactee)
        {
            StartDialogue(EntryNode);
            return true;
        }
    }
}
