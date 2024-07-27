using System.Collections.Generic;
using Sirenix.OdinInspector;
using SunsetSystems.Equipment;
using SunsetSystems.Equipment.UI;
using SunsetSystems.Inventory;
using SunsetSystems.Inventory.Data;
using SunsetSystems.Inventory.UI;
using SunsetSystems.Party;
using SunsetSystems.UI.Utils;
using UnityEngine;

namespace SunsetSystems.UI
{
    public class InventoryUIParent : MonoBehaviour
    {
        [SerializeField, Required]
        private InventoryContentsUpdater _inventoryContentsUpdater;
        [SerializeField, Required]
        private EquipmentContentsUpdater _equipmentContentsUpdater;

        private bool _isDirty;

        private void OnEnable()
        {
            string characterKey = CharacterSelector.SelectedCharacterKey;
            UpdateEquipment(characterKey);
            UpdateInventory();
            InventoryManager.OnItemAcquired += MarkDirty;
            InventoryManager.OnItemLost += MarkDirty;
        }

        private void OnDisable()
        {
            InventoryManager.OnItemAcquired -= MarkDirty;
            InventoryManager.OnItemLost -= MarkDirty;
        }

        private void Update()
        {
            if (_isDirty)
            {
                Refresh();
                _isDirty = false;
            }
        }

        public void MarkDirty(IBaseItem _) => MarkDirty();

        public void MarkDirty() => _isDirty = true;

        private void Refresh()
        {
            string characterKey = CharacterSelector.SelectedCharacterKey;
            UpdateEquipment(characterKey);
            UpdateInventory();
        }

        private void UpdateEquipment(string characterKey)
        {
            if (PartyManager.Instance.IsRecruitedMember(characterKey))
            {
                List<IGameDataProvider<IEquipmentSlot>> slots = new();
                var memberEquipment = PartyManager.Instance.GetPartyMemberByID(characterKey).References.EquipmentManager;
                if (memberEquipment != null)
                {
                    foreach (EquipmentSlotID key in memberEquipment.EquipmentSlots.Keys)
                    {
                        slots.Add(memberEquipment.EquipmentSlots[key]);
                    }
                }
                _equipmentContentsUpdater.UpdateViews(slots);
            }
        }

        private void UpdateInventory()
        {
            List<IGameDataProvider<InventoryEntry>> items = new();
            foreach (InventoryEntry entry in InventoryManager.Instance.GetPlayerInventoryContents())
            {
                items.Add(entry);
            }
            _inventoryContentsUpdater.UpdateViews(items);
        }
    }
}
