using Sirenix.OdinInspector;
using SunsetSystems.Entities.Characters.Actions;
using SunsetSystems.Journal;
using UnityEngine;

namespace SunsetSystems.Entities.Interactable
{
    public class QuestTrigger : SerializedMonoBehaviour, IQuestTrigger, IInteractionHandler
    {
        [SerializeReference]
        private Quest _myQuest;

        public bool HandleInteraction(IActionPerformer interactee)
        {
            return TriggerQuest(_myQuest);
        }

        public bool TriggerQuest(Quest questToTrigger)
        {
            Debug.Log("Starting quest " + questToTrigger.Name);
            return QuestJournal.Instance.BeginQuest(questToTrigger.DatabaseID);
        }
    }
}
