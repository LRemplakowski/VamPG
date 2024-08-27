using Sirenix.OdinInspector;
using SunsetSystems.Inventory;
using SunsetSystems.Inventory.Data;
using SunsetSystems.UI.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace SunsetSystems.Equipment.UI
{
    public class EquipmentSlotDisplay : Button, IUserInterfaceView<IEquipmentSlot>
    {
        [SerializeField, ReadOnly]
        private IEquipmentSlot _cachedSlotData;
        [SerializeField]
        private Image _itemIcon;

        public void UpdateView(IUserInfertaceDataProvider<IEquipmentSlot> dataProvider)
        {
            _cachedSlotData = dataProvider.UIData;
            IEquipableItem itemInSlot = _cachedSlotData.GetEquippedItem();
            if (itemInSlot != null)
            {
                _itemIcon.sprite = itemInSlot.Icon;
                _itemIcon.gameObject.SetActive(true);
            }
            else
            {
                _itemIcon.gameObject.SetActive(false);
            }
        }

        public void UnequipItemFromSlot()
        {
            if (CanUnequipItem(_cachedSlotData))
            {
                _cachedSlotData.TryUnequipItem(out var unequipped);
                InventoryManager.Instance.GiveItemToPlayer(unequipped, postLogMessage: false);
            }
        }

        private static bool CanUnequipItem(IEquipmentSlot slot)
        {
            if (slot == null)
                return false;
            var item = slot.GetEquippedItem();
            return item != null && item.CanBeRemoved && !item.IsDefaultItem;
        }
    }
}
