using NaughtyAttributes;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Inventory;
using SunsetSystems.Inventory.UI;
using SunsetSystems.UI.Utils;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SunsetSystems.UI
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField, Required]
        private InventoryContentsUpdater _inventoryContentsUpdater;
        [SerializeField, Required]
        private EquipmentContentsUpdater _equipmentContentsUpdater;

        private IInventoryManager _inventoryManager;
        private ICharacterSelector _characterSelector;

        [Inject]
        public void InjectDependencies(IInventoryManager inventoryManager, ICharacterSelector characterSelector)
        {
            _inventoryManager = inventoryManager;
            _characterSelector = characterSelector;
        }

        private void OnEnable()
        {
            string characterKey = _characterSelector.SelectedCharacterKey;
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
            List<IGameDataProvider<InventoryEntry>> items = new();
            items.AddRange(_inventoryManager.GetPlayerInventoryContents());
            _inventoryContentsUpdater.UpdateViews(items);
        }

        private void UpdateInventory(string characterKey)
        {
            List<IGameDataProvider<EquipmentSlot>> slots = new();
            if (_inventoryManager.TryGetEquipmentData(_characterSelector.SelectedCharacterKey, out EquipmentData data))
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
