using SunsetSystems.Journal;
using UnityEngine;
using Zenject;

namespace SunsetSystems.Entities.Interactable
{
    public class QuestTriggerEntity : InteractableEntity, IQuestTrigger
    {
        [SerializeReference]
        private Quest _myQuest;
        public Quest MyQuest => _myQuest;

        private IQuestJournal _questJournal;

        [Inject]
        public void InjectDependencies(IQuestJournal questJournal)
        {
            _questJournal = questJournal;
        }

        public void TriggerQuest()
        {
            _questJournal.BeginQuest(_myQuest.ID);
        }

        protected override void HandleInteraction()
        {
            TriggerQuest();
        }
    }
}
