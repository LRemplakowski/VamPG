using NaughtyAttributes;
using Redcode.Awaiting;
using SunsetSystems.Dialogue;
using SunsetSystems.LevelManagement;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;
using Zenject;

namespace SunsetSystems.Entities.Interactable
{
    public class DialogueEntity : InteractableEntity
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

        private IDialogueManager _dialogueManager;

        [Inject]
        public void InjectDependencies(IDialogueManager dialogueManager)
        {
            _dialogueManager = dialogueManager;
        }

        protected async override void HandleInteraction()
        {
            if (_fadeOutBeforeDialogue)
            {
                SceneLoadingUIManager fade = this.FindFirstComponentWithTag<SceneLoadingUIManager>(TagConstants.SCENE_LOADING_UI);
                await fade.DoFadeOutAsync(_fadeTimes.x);
                _onAfterFadeout?.Invoke();
                await new WaitForSecondsRealtime(_fadeTimes.y);
                await fade.DoFadeInAsync(_fadeTimes.z);
            }
            _dialogueManager.StartDialogue(EntryNode, _dialogueProject);
        }
    }
}
