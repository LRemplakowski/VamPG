using NaughtyAttributes;
using Redcode.Awaiting;
using SunsetSystems.Dialogue;
using SunsetSystems.Persistence;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

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

        public override object GetPersistenceData()
        {
            DialogueEntityPersistenceData persistenceData = new(base.GetPersistenceData() as InteractableEntityPersistenceData);
            persistenceData.EntryNode = EntryNode;
            return persistenceData;
        }

        public override void InjectPersistenceData(object data)
        {
            base.InjectPersistenceData(data);
            DialogueEntityPersistenceData persistenceData = data as DialogueEntityPersistenceData;
            EntryNode = persistenceData?.EntryNode;
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
            DialogueManager.Instance.StartDialogue(EntryNode, _dialogueProject);
        }

        protected class DialogueEntityPersistenceData : InteractableEntityPersistenceData
        {
            public string EntryNode;

            public DialogueEntityPersistenceData(InteractableEntityPersistenceData persistenceData)
            {
                Interactable = persistenceData.Interactable;
                GameObjectActive = persistenceData.GameObjectActive;
            }

            public DialogueEntityPersistenceData()
            {

            }
        }
    }
}
