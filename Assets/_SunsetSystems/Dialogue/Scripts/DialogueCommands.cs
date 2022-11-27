using SunsetSystems.Journal;
using System.Collections.Generic;
using System.Linq;
using Yarn.Unity;

namespace SunsetSystems.Dialogue
{
    public static class DialogueCommands
    {
        [YarnCommand("StartQuest")]
        public static void StartQuest(string questID)
        {
            QuestJournal.Instance.BeginQuest(questID);
        }

        [YarnCommand("CompleteObjective")]
        public static void CompleteObjective(string questID, string objectiveID)
        {
            if (QuestJournal.Instance.GetCurrentObjectives(questID, out List<Objective> objectives))
            {
                objectives.Find(o => o.ID.Equals(objectiveID))?.Complete();
            }
        }
    }
}
