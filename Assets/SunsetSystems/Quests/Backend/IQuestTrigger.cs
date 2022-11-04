using UnityEngine;

namespace SunsetSystems.Journal
{
    public interface IQuestTrigger
    {
        Quest MyQuest { get; }

        void TriggerQuest()
        {
            Debug.Log("Starting quest " + MyQuest.QuestData.Name);
            QuestJournal.Instance.BeginQuest(MyQuest.ID);
        }
    }
}
