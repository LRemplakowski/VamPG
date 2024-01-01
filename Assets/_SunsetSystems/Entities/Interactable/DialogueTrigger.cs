using Sirenix.OdinInspector;
using Redcode.Awaiting;
using SunsetSystems.Dialogue;
using SunsetSystems.Dialogue.Interfaces;
using SunsetSystems.Persistence;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;
using SunsetSystems.Entities.Characters.Actions;

namespace SunsetSystems.Entities.Interactable
{
    public class DialogueTrigger : PersistentEntity, IInteractionHandler, IDialogueSource
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
            DialogueTriggerPersistenceData persistenceData = new(base.GetPersistenceData() as DialogueTriggerPersistenceData);
            persistenceData.EntryNode = EntryNode;
            return persistenceData;
        }

        public override void InjectPersistenceData(object data)
        {
            base.InjectPersistenceData(data);
            DialogueTriggerPersistenceData persistenceData = data as DialogueTriggerPersistenceData;
            EntryNode = persistenceData?.EntryNode;
        }

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

        protected class DialogueTriggerPersistenceData : PersistenceData
        {
            public string EntryNode;

            public DialogueTriggerPersistenceData(PersistenceData defaultData)
            {
                this.GameObjectActive = defaultData.GameObjectActive;
            }

            public DialogueTriggerPersistenceData()
            {

            }
        }
    }
}
