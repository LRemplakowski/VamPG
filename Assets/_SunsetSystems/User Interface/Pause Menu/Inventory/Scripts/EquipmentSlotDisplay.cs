using NaughtyAttributes;
using SunsetSystems.Inventory.Data;
using SunsetSystems.UI;
using SunsetSystems.UI.Utils;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SunsetSystems.Inventory.UI
{
    public class EquipmentSlotDisplay : Button, IUserInterfaceView<EquipmentSlot>
    {
        [SerializeField, ReadOnly]
        private EquipmentSlot _cachedSlotData;
        [SerializeField]
        private Image _itemIcon;

        private ICharacterSelector _characterSelector;
        private IInventoryManager _inventoryManager;

        [Inject]
        public void InjectDependencies(ICharacterSelector characterSelector, IInventoryManager inventoryManager)
        {
            _characterSelector = characterSelector;
            _inventoryManager = inventoryManager;
        }

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
                _inventoryManager.TryUnequipItemFromSlot(_characterSelector.SelectedCharacterKey, _cachedSlotData.ID);
            }
        }
    }
}
