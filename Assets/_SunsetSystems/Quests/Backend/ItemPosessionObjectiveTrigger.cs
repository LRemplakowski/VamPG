using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using SunsetSystems.Inventory;
using SunsetSystems.Inventory.Data;
using SunsetSystems.Party;
using UnityEngine;

namespace SunsetSystems.Journal
{
    public class ItemPosessionObjectiveTrigger : SerializedMonoBehaviour
    {
        [SerializeField, Required]
        private Objective objectiveToTrigger;
        [OdinSerialize, Required, HideReferenceObjectPicker]
        private List<ItemObjectiveData> requiredItems = new();

        private void Start()
        {
            Objective.OnObjectiveActive += StartHandlingObjective;
            Objective.OnObjectiveFailed += StopHandlingObjective;
            Objective.OnObjectiveCompleted += StopHandlingObjective;
        }


        private void StartHandlingObjective(Objective obj)
        {
            if (obj.DatabaseID != objectiveToTrigger.DatabaseID)
                return;
            bool hasRequiredItems = false;
            if (requiredItems.Count > 0)
                hasRequiredItems = true;
            foreach (ItemObjectiveData itemData in requiredItems)
            {
                hasRequiredItems &= itemData.HasRequiredItems();
            }
            if (hasRequiredItems)
            {
                objectiveToTrigger.Complete();
            }
            else
            {
                InventoryManager.OnItemAcquired += OnItemAddedToPlayerInventory;
            }
        }

        private void OnItemAddedToPlayerInventory(IBaseItem item)
        {
            if (requiredItems.Any(requiredItemEntry => requiredItemEntry.GetRequiredItems().Any(required => required.DatabaseID == item.DatabaseID)) is false)
                return;
            bool hasRequiredItems = false;
            if (requiredItems.Count > 0)
                hasRequiredItems = true;
            foreach (ItemObjectiveData itemData in requiredItems)
            {
                hasRequiredItems &= itemData.HasRequiredItems();
                if (hasRequiredItems is false)
                    break;
            }
            if (hasRequiredItems)
            {
                objectiveToTrigger.Complete();
            }
        }

        private void StopHandlingObjective(Objective obj)
        {
            Objective.OnObjectiveActive -= StartHandlingObjective;
            Objective.OnObjectiveFailed -= StopHandlingObjective;
            Objective.OnObjectiveCompleted -= StopHandlingObjective;
            InventoryManager.OnItemAcquired -= OnItemAddedToPlayerInventory;
        }

        [Serializable]
        private class ItemObjectiveData
        {
            [OdinSerialize, Required]
            private List<IBaseItem> requiredItems = new();
            [SerializeField, Min(1)]
            private int requiredCount = 1;
            [SerializeField]
            private ItemCountLogic countingLogic;

            public IEnumerable<IBaseItem> GetRequiredItems()
            {
                return requiredItems.AsEnumerable();
            }

            public bool HasRequiredItems()
            {
                int itemCount = 0;
                foreach (IBaseItem item in requiredItems)
                {
                    bool hasRequiredItems;
                    if (InventoryManager.Instance.GetInventoryContainsItemWithReadableID(item.ReadableID, out int oneTypeCount))
                    {
                        switch (countingLogic)
                        {
                            case ItemCountLogic.CountOneType:
                                hasRequiredItems = oneTypeCount >= requiredCount;
                                if (hasRequiredItems)
                                    return true;
                                break;
                            case ItemCountLogic.CountAllTypes:
                                itemCount += oneTypeCount;
                                hasRequiredItems = itemCount >= requiredCount;
                                if (hasRequiredItems)
                                    return true;
                                break;
                        }
                    }
                    else if (item is IEquipableItem equipable)
                    {
                        var equipments = PartyManager.Instance.ActiveParty.Select(member => member.References.EquipmentManager);
                        oneTypeCount = 0;
                        foreach (var eq in equipments)
                        {
                            if (eq.IsItemEquipped(equipable))
                            {
                                switch (countingLogic)
                                {
                                    case ItemCountLogic.CountOneType:
                                        oneTypeCount += 1;
                                        hasRequiredItems = oneTypeCount >= requiredCount;
                                        if (hasRequiredItems)
                                            return true;
                                        break;
                                    case ItemCountLogic.CountAllTypes:
                                        itemCount += 1;
                                        hasRequiredItems = itemCount >= requiredCount;
                                        if (hasRequiredItems)
                                            return true;
                                        break;
                                }
                            }
                        }
                    }
                }
                return false;
            }

            private enum ItemCountLogic
            {
                CountOneType, CountAllTypes
            }
        }
    }
}
