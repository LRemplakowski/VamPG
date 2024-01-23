using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using SunsetSystems.Inventory;
using SunsetSystems.Inventory.Data;
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
            objectiveToTrigger.OnObjectiveActive += StartHandlingObjective;
            objectiveToTrigger.OnObjectiveInactive += StopHandlingObjective;
            objectiveToTrigger.OnObjectiveCompleted += StopHandlingObjective;
        }

        private void StartHandlingObjective(Objective obj)
        {
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
                InventoryManager.PlayerInventory.OnItemAdded += OnItemAddedToPlayerInventory;
            }
        }

        private void OnItemAddedToPlayerInventory(InventoryEntry itemEntry)
        {
            if (requiredItems.Any(requiredItemEntry => requiredItemEntry.GetRequiredItems().Contains(itemEntry._item)) is false)
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
            objectiveToTrigger.OnObjectiveActive -= StartHandlingObjective;
            objectiveToTrigger.OnObjectiveInactive -= StopHandlingObjective;
            objectiveToTrigger.OnObjectiveCompleted -= StopHandlingObjective;
            InventoryManager.PlayerInventory.OnItemAdded -= OnItemAddedToPlayerInventory;
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
                    if (InventoryManager.Instance.GetInventoryContainsItemWithReadableID(item.ReadableID, out int count))
                    {
                        bool hasRequiredItems;
                        switch (countingLogic)
                        {
                            case ItemCountLogic.CountOneType:
                                hasRequiredItems = count >= requiredCount;
                                if (hasRequiredItems)
                                    return true;
                                break;
                            case ItemCountLogic.CountAllTypes:
                                itemCount += count;
                                hasRequiredItems = itemCount >= requiredCount;
                                if (hasRequiredItems)
                                    return true;
                                break;
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
