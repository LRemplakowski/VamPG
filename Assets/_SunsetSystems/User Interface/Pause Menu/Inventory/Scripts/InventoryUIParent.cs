using Sirenix.OdinInspector;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Inventory;
using SunsetSystems.Inventory.Data;
using SunsetSystems.Inventory.UI;
using SunsetSystems.Party;
using SunsetSystems.UI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SunsetSystems.UI
{
    public class InventoryUIParent : MonoBehaviour
    {
        [SerializeField, Required]
        private InventoryContentsUpdater _inventoryContentsUpdater;
        [SerializeField, Required]
        private EquipmentContentsUpdater _equipmentContentsUpdater;

        private void OnEnable()
        {
            string characterKey = CharacterSelector.SelectedCharacterKey;
            UpdateEquipment(characterKey);
            UpdateInventory(characterKey);

            InventoryManager.ItemEquipped += UpdateInventory;
            InventoryManager.ItemEquipped += UpdateEquipment;
            InventoryManager.ItemUnequipped += UpdateInventory;
            InventoryManager.ItemUnequipped += UpdateEquipment;
        }

        private void OnDisable()
        {
            InventoryManager.ItemEquipped -= UpdateInventory;
            InventoryManager.ItemEquipped -= UpdateEquipment;
            InventoryManager.ItemUnequipped -= UpdateInventory;
            InventoryManager.ItemUnequipped -= UpdateEquipment;
        }

        private void UpdateEquipment(string characterKey)
        {
            if (PartyManager.Instance.IsRecruitedMember(characterKey))
            {
                List<IGameDataProvider<InventoryEntry>> items = new();
                items.AddRange(InventoryManager.PlayerInventory.Contents);
                _inventoryContentsUpdater.UpdateViews(items);
            }
        }

        private void UpdateInventory(string characterKey)
        {
            if (PartyManager.Instance.IsRecruitedMember(characterKey))
            {
                List<IGameDataProvider<EquipmentSlot>> slots = new();
                if (InventoryManager.TryGetEquipmentData(CharacterSelector.SelectedCharacterKey, out EquipmentData data))
                {
                    foreach (string key in data.EquipmentSlots.Keys)
                    {
                        slots.Add(data.EquipmentSlots[key]);
                    }
                }
                _equipmentContentsUpdater.UpdateViews(slots);
            }
        }
    }
}
