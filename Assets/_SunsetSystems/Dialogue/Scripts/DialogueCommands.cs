using SunsetSystems.Audio;
using SunsetSystems.Core.Database;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Inventory;
using SunsetSystems.Inventory.Data;
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
            //ICreature character = PartyManager.Instance.GetPartyMemberByID(characterID);
            //character.StatsManager.TryUseBlood(value);
            throw new NotImplementedException();
        }

        [YarnCommand("DecreaseHunger")]
        public static void DecreaseHunger(string characterID, int value)
        {
            //Creature character = PartyManager.Instance.GetPartyMemberByID(characterID);
            //character.StatsManager.RegainBlood(value);
            throw new NotImplementedException();
        }

        [YarnCommand("AddMoney")]
        public static void AddMoney(float value)
        {
            InventoryManager.Instance.AddMoney(value);
        }

        [YarnCommand("ModifyInfluence")]
        public static void ModifyInfluence(int amount, string companionID)
        {
            Debug.LogError("Modify that influence, you dummy");
        }

        [YarnCommand("RemoveMoney")]
        public static void RemoveMoney(float value)
        {
            if (InventoryManager.Instance.TryRemoveMoney(value) == false)
                throw new ArgumentException($"Money amount {value} is greater than current funds! Check money amount before removing money!");
        }

        [YarnCommand("PlaySFX")]
        public static void PlaySFX(string clipName)
        {
            AudioManager.Instance.PlaySFXOneShot(clipName);
        }

        [YarnCommand("GiveItem")]
        public static void GiveItem(string itemID)
        {
            if (ItemDatabase.Instance.TryGetEntryByReadableID(itemID, out IBaseItem item))
                InventoryManager.Instance.GiveItemToPlayer(new(item));
            else
                Debug.LogError($"Dialogue command GiveItem: Could not find item {itemID} in the ItemDatabase instance!");
        }

        [YarnCommand("GiveItemCount")]
        public static void GiveItemCount(string itemID, int count)
        {
            if (count <= 0)
                throw new ArgumentException("Item count cannot be less than 1!");
            if (ItemDatabase.Instance.TryGetEntryByReadableID(itemID, out IBaseItem item))
                InventoryManager.Instance.GiveItemToPlayer(new(item, count));
            else
                Debug.LogError($"Dialogue command GiveItemCount: Could not find item {itemID} in the ItemDatabase instance!");
        }

        [YarnCommand("DealDamage")]
        public static void DealDamage(string characterID, int damage, string damageType)
        {
            throw new NotImplementedException();
        }

        [YarnCommand("DecreaseWillpower")]
        public static void DecreaseWillpower(string characterID, int value)
        {
            throw new NotImplementedException();
        }
    }
}
