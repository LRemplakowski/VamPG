using SunsetSystems.Entities.Characters;
using SunsetSystems.Inventory;
using SunsetSystems.Journal;
using SunsetSystems.Party;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Yarn.Unity;

namespace SunsetSystems.Dialogue
{
    public static class DialogueCommands
    {
        [YarnCommand("StartQuest")]
        public static void StartQuest(string readableID)
        {
            QuestJournal.Instance.BeginQuestByReadableID(readableID);
        }

        [YarnCommand("CompleteObjective")]
        public static void CompleteObjective(string readableQuestID, string objectiveID)
        {
            if (QuestJournal.Instance.TryGetTrackedObjectiveByReadableID(readableQuestID, objectiveID, out Objective objective))
            {
                objective.Complete();
            }
            else
            {
                Debug.LogError($"Dialogue tried to completed objective {objectiveID} in quest {readableQuestID} but that objective is not currently active!");
            }
        }

        [YarnCommand("FailObjective")]
        public static void FailObjective(string readableQuestID, string objectiveID)
        {
            if (QuestJournal.Instance.TryGetTrackedObjectiveByReadableID(readableQuestID, objectiveID, out Objective objective))
            {
                objective.MakeInactive();
            }
            else
            {
                Debug.LogError($"Dialogue tried to fail objective {objectiveID} in quest {readableQuestID} but that objective is not currently active!");
            }
        }

        [YarnCommand("IncreaseHunger")]
        public static void IncreaseHunger(string characterID, int value)
        {
            Creature character = PartyManager.Instance.GetPartyMemberByID(characterID);
            character.StatsManager.TryUseBlood(value);
        }

        [YarnCommand("DecreaseHunger")]
        public static void DecreaseHunger(string characterID, int value)
        {
            Creature character = PartyManager.Instance.GetPartyMemberByID(characterID);
            character.StatsManager.RegainBlood(value);
        }

        [YarnCommand("OverrideSpeakerPortrait")]
        public static void OverrideDialoguePortrait(string ownerID)
        {
            DialogueManager.Instance.OverrideSpeakerPortrait(ownerID);
        }

        [YarnCommand("ClearPortraitOverride")]
        public static void ClearDialoguePortraitOverride()
        {
            DialogueManager.Instance.ClearSpeakerPortraitOverride();
        }

        [YarnCommand("SetDefaultSpeakerPortrait")]
        public static void SetDefaultSpeakerPortrait(string speakerID)
        {
            DialogueManager.Instance.SetDefaultSpeakerPortrait(speakerID);
        }

        [YarnCommand("AddMoney")]
        public static void AddMoney(float value)
        {
            InventoryManager.Instance.AddMoney(value);
        }

        [YarnCommand("RemoveMoney")]
        public static void RemoveMoney(float value)
        {
            if (InventoryManager.Instance.TryRemoveMoney(value) == false)
                throw new ArgumentException($"Money amount {value} is greater than current funds! Check money amount before removing money!");
        }
    }
}
