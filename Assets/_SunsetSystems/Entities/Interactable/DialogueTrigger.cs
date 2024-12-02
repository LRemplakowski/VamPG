using Sirenix.OdinInspector;
using SunsetSystems.Dialogue;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;
using SunsetSystems.ActionSystem;
using UltEvents;
using SunsetSystems.Core.SceneLoading.UI;
using System.Threading.Tasks;

namespace SunsetSystems.Entities.Interactable
{
    public class DialogueTrigger : SerializedMonoBehaviour, IInteractionHandler, IDialogueSource
    {
        [SerializeField]
        private bool _fadeOutBeforeDialogue;
        [SerializeField]
        private bool _fadeOutAfterDialogue;
        [InfoBox("XYZ represent fade out timings.\nX - Fading out time\nY - Time screen stays faded out\nZ - Fading in time")]
        [Space]
        [SerializeField, ShowIf("@this._fadeOutBeforeDialogue == true || this._fadeOutAfterDialogue == true")]
        private Vector3 _fadeTimes = Vector3.one;
        [SerializeField, ShowIf("_fadeOutBeforeDialogue")]
        private UnityEvent _onAfterFadeout;
        [SerializeField, ShowIf("_fadeOutAfterDialogue")]
        private UnityEvent _onAfterDialogueFadeout;
        [SerializeField]
        private YarnProject _dialogueProject;
        [field: SerializeField]
        public string EntryNode { get; set; }

        public UltEvent OnDialogueStarted;
        public UltEvent OnDialogueFinished;
        public UltEvent<string> OnNodeStarted;
        public UltEvent<string> OnNodeFinished;

        [Button]
        public async void StartDialogue()
        {
            if (_fadeOutBeforeDialogue)
            {
                var fade = SceneLoadingUIManager.Instance;
                await fade.DoFadeOutAsync(_fadeTimes.x);
                _onAfterFadeout?.Invoke();
                await Task.Delay(Mathf.RoundToInt(_fadeTimes.y * 1000));
                await fade.DoFadeInAsync(_fadeTimes.z);
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

        private async void PropagateDialogueFinished()
        {
            OnDialogueFinished?.InvokeSafe();
            if (_fadeOutAfterDialogue)
            {
                var fade = SceneLoadingUIManager.Instance;
                await fade.DoFadeOutAsync(_fadeTimes.x);
                _onAfterDialogueFadeout?.Invoke();
                await Task.Delay(Mathf.RoundToInt(_fadeTimes.y * 1000));
                await fade.DoFadeInAsync(_fadeTimes.z);
            }
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
