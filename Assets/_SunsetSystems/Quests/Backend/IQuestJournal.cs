using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Journal
{
    public interface IQuestJournal
    {
        bool IsQuestCompleted(string questID);
        bool BeginQuest(string questID);
        bool BeginQuestByReadableID(string readableID);
        bool GetCurrentObjectives(string questID, out List<Objective> objectives);
        bool CompleteObjective(string questID, string objectiveID);
        bool FailObjective(string questID, string objectiveID);

        List<Quest> ActiveQuests { get; }
        List<Quest> MainQuests { get; }
        List<Quest> SideQuests { get; }
        List<Quest> CaseQuests { get; }
        List<Quest> CompletedQuests { get; }
    }
}
