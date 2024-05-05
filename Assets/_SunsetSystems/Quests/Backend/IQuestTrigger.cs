using UnityEngine;

namespace SunsetSystems.Journal
{
    public interface IQuestTrigger
    {
        bool TriggerQuest(Quest questToTrigger);
    }
}
