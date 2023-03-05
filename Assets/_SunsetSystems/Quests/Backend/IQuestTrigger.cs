using UnityEngine;

namespace SunsetSystems.Journal
{
    public interface IQuestTrigger
    {
        Quest MyQuest { get; }

        void TriggerQuest();
    }
}
