using Sirenix.OdinInspector;
using SunsetSystems.Dialogue;
using SunsetSystems.Dialogue.Interfaces;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;
using SunsetSystems.Entities.Characters.Actions;
using UltEvents;

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

        public UltEvent OnDialogueStarted;
        public UltEvent OnDialogueFinished;
        public UltEvent<string> OnNodeStarted;
        public UltEvent<string> OnNodeFinished;

        public void StartDialogue()
        {
            if (_fadeOutBeforeDialogue)
            {
                _onAfterFadeout?.Invoke();
            }
            SubscribeToDialogueEvents();
            DialogueManager.Instance.StartDialogue(EntryNode, _dialogueProject);
        }

        private void SubscribeToDialogueEvents()
        {
            DialogueManager.Instance.OnDialogueStarted.AddListener(PropagateDialogueStarted);
            DialogueManager.Instance.OnDialogueFinished.AddListener(PropagateDialogueFinished);
            DialogueManager.Instance.OnNodeStarted.AddListener(PropagateNodeStarted);
            DialogueManager.Instance.OnNodeFinished.AddListener(PropagateNodeFinished);
        }

        private void UnsubscribeFromDialogueEvents()
        {
            DialogueManager.Instance.OnDialogueStarted.RemoveListener(PropagateDialogueStarted);
            DialogueManager.Instance.OnDialogueFinished.RemoveListener(PropagateDialogueFinished);
            DialogueManager.Instance.OnNodeStarted.RemoveListener(PropagateNodeStarted);
            DialogueManager.Instance.OnNodeFinished.RemoveListener(PropagateNodeFinished);
        }

        private void PropagateDialogueStarted()
        {
            OnDialogueStarted?.InvokeSafe();
        }

        private void PropagateDialogueFinished()
        {
            OnDialogueFinished?.InvokeSafe();
            UnsubscribeFromDialogueEvents();
        }

        private void PropagateNodeStarted(string arg)
        {
            OnNodeStarted?.InvokeSafe(arg);
        }

        private void PropagateNodeFinished(string arg)
        {
            OnNodeFinished?.InvokeSafe(arg);
        }

        public bool HandleInteraction(IActionPerformer interactee)
        {
            StartDialogue();
            return true;
        }
    }
}
