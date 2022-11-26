using SunsetSystems.Journal;
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
            if (QuestJournal.Instance.GetCurrentObjective(questID, out Objective objective))
            {
                if (objective.ID.Equals(objectiveID))
                {
                    objective.Complete();
                }
            }
        }
    }
}
