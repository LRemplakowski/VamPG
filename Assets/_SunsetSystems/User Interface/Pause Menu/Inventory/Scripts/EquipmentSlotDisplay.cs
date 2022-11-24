using NaughtyAttributes;
using SunsetSystems.Inventory.Data;
using SunsetSystems.UI.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SunsetSystems.Inventory.UI
{
    public class EquipmentSlotDisplay : Button, IUserInterfaceView<EquipmentSlot>
    {
        [SerializeField, ReadOnly]
        private EquipmentSlot _cachedSlotData;
        [SerializeField]
        private Image _itemIcon;

        public void UpdateView(IGameDataProvider<EquipmentSlot> dataProvider)
        {
            _cachedSlotData = dataProvider.Data;
            EquipableItem itemInSlot = _cachedSlotData.GetEquippedItem();
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
            if (_cachedSlotData.GetEquippedItem() != null)
            {
                InventoryManager.TryUnequipItemFromSlot(CharacterSelector.SelectedCharacterKey, _cachedSlotData.ID);
            }
        }
    }
}
