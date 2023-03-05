using SunsetSystems.Audio;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Inventory;
using SunsetSystems.Journal;
using SunsetSystems.Party;
using System;
using UnityEngine;
using Yarn.Unity;
using Zenject;

namespace SunsetSystems.Dialogue
{
    public static class DialogueCommands
    {
        [Inject]
        private static IQuestJournal questJournal;
        [Inject]
        private static IPartyManager partyManager;
        [Inject]
        private static IInventoryManager inventoryManager;
        [Inject]
        private static IAudioManager audioManager;

        [YarnCommand("StartQuest")]
        public static void StartQuest(string readableID)
        {
            questJournal.BeginQuestByReadableID(readableID);
        }

        [YarnCommand("CompleteObjective")]
        public static void CompleteObjective(string readableQuestID, string objectiveID)
        {
            questJournal.CompleteObjective(readableQuestID, objectiveID);
        }

        [YarnCommand("FailObjective")]
        public static void FailObjective(string readableQuestID, string objectiveID)
        {
            questJournal.FailObjective(readableQuestID, objectiveID);
        }

        [YarnCommand("IncreaseHunger")]
        public static void IncreaseHunger(string characterID, int value)
        {
            Creature character = partyManager.GetPartyMemberByID(characterID);
            character.StatsManager.TryUseBlood(value);
        }

        [YarnCommand("DecreaseHunger")]
        public static void DecreaseHunger(string characterID, int value)
        {
            Creature character = partyManager.GetPartyMemberByID(characterID);
            character.StatsManager.RegainBlood(value);
        }

        [YarnCommand("AddMoney")]
        public static void AddMoney(float value)
        {
            inventoryManager.AddMoney(Mathf.RoundToInt(value));
        }

        [YarnCommand("ModifyInfluence")]
        public static void ModifyInfluence(int amount, string companionID)
        {
            Debug.LogError("Modify that influence, you dummy");
        }

        [YarnCommand("RemoveMoney")]
        public static void RemoveMoney(float value)
        {
            if (inventoryManager.TryRemoveMoney(Mathf.RoundToInt(value)) == false)
                throw new ArgumentException($"Money amount {value} is greater than current funds! Check money amount before removing money!");
        }

        [YarnCommand("PlaySFX")]
        public static void PlaySFX(string clipName)
        {
            audioManager.PlaySFXOneShot(clipName);
        }
    }
}
