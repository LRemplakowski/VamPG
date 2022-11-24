using SunsetSystems.Journal;
using SunsetSystems.Party;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

namespace SunsetSystems.Dialogue
{
    public static class DialogueFunctions
    {
        [YarnFunction]
        public static string GetPCName()
        {
            return PartyManager.MainCharacter.Data.FullName;
        }

        [YarnCommand("StartQuest")]
        public static void StartQuest(string questID)
        {
            QuestJournal.Instance.BeginQuest(questID);
        }
    }
}
