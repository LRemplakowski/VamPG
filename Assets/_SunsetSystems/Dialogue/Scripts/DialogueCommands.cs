using SunsetSystems.Audio;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Inventory;
using SunsetSystems.Inventory.Data;
using SunsetSystems.Journal;
using SunsetSystems.Party;
using System;
using UnityEngine;

namespace SunsetSystems.Dialogue
{
    public static class DialogueCommands
    {
        public static void StartQuest(string readableID)
        {
            QuestJournal.Instance.BeginQuestByReadableID(readableID);
        }

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

        public static void IncreaseHunger(string characterID, int value)
        {
            Creature character = PartyManager.Instance.GetPartyMemberByID(characterID);
            character.StatsManager.TryUseBlood(value);
        }

        public static void DecreaseHunger(string characterID, int value)
        {
            Creature character = PartyManager.Instance.GetPartyMemberByID(characterID);
            character.StatsManager.RegainBlood(value);
        }

        public static void AddMoney(float value)
        {
            InventoryManager.Instance.AddMoney(value);
        }

        public static void ModifyInfluence(int amount, string companionID)
        {
            Debug.LogError("Modify that influence, you dummy");
        }

        public static void RemoveMoney(float value)
        {
            if (InventoryManager.Instance.TryRemoveMoney(value) == false)
                throw new ArgumentException($"Money amount {value} is greater than current funds! Check money amount before removing money!");
        }

        public static void PlaySFX(string clipName)
        {
            AudioManager.Instance.PlaySFXOneShot(clipName);
        }

        public static void GiveItem(string itemID)
        {
            if (ItemDatabase.Instance.TryGetEntryByReadableID(itemID, out BaseItem item))
                InventoryManager.Instance.GiveItemToPlayer(new(item));
            else
                Debug.LogError($"Dialogue command GiveItem: Could not find item {itemID} in the ItemDatabase instance!");
        }

        public static void GiveItemCount(string itemID, int count)
        {
            if (count <= 0)
                throw new ArgumentException("Item count cannot be less than 1!");
            if (ItemDatabase.Instance.TryGetEntryByReadableID(itemID, out BaseItem item))
                InventoryManager.Instance.GiveItemToPlayer(new(item, count));
            else
                Debug.LogError($"Dialogue command GiveItemCount: Could not find item {itemID} in the ItemDatabase instance!");
        }

        public static void DealDamage(string characterID, int damage, string damageType)
        {
            throw new NotImplementedException();
        }

        public static void DecreaseWillpower(string characterID, int value)
        {
            throw new NotImplementedException();
        }
    }
}
