using NaughtyAttributes;
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
            UpdateEquipment(characterKey, null);
            UpdateInventory(characterKey, null);

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

        private void UpdateEquipment(string characterKey, EquipableItem item)
        {
            if (PartyManager.Instance.IsRecruitedMember(characterKey))
            {
                List<IGameDataProvider<InventoryEntry>> items = new();
                items.AddRange(InventoryManager.PlayerInventory.Contents);
                _inventoryContentsUpdater.UpdateViews(items);
            }
        }

        private void UpdateInventory(string characterKey, EquipableItem item)
        {
            if (PartyManager.Instance.IsRecruitedMember(characterKey))
            {
                List<IGameDataProvider<EquipmentSlot>> slots = new();
                InventoryManager.TryGetEquipmentData(CharacterSelector.SelectedCharacterKey, out EquipmentData data);
                slots.AddRange(data.equipmentSlots.Values);
                _equipmentContentsUpdater.UpdateViews(slots);
            }
        }
    }
}
