using Sirenix.OdinInspector;
using SunsetSystems.Core.AddressableManagement;
using SunsetSystems.Inventory.Data;
using SunsetSystems.UI.Utils;
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace SunsetSystems.Equipment.UI
{
    public class EquipmentSlotDisplay : Button, IUserInterfaceView<IEquipmentSlot>
    {
        [SerializeField, ReadOnly]
        private IEquipmentSlot _cachedSlotData;
        [SerializeField]
        private Image _itemIcon;

        public void UpdateView(IGameDataProvider<IEquipmentSlot> dataProvider)
        {
            //if (lastLoadedSprite != null)
            //    AddressableManager.Instance.ReleaseAsset(lastLoadedSprite);
            _cachedSlotData = dataProvider.Data;
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
            if (_cachedSlotData.GetEquippedItem() != null)
            {
                throw new NotImplementedException();
            }
        }
    }
}
