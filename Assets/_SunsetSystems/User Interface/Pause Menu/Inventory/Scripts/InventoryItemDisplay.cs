using Sirenix.OdinInspector;
using SunsetSystems.Core.AddressableManagement;
using SunsetSystems.Inventory;
using SunsetSystems.Inventory.Data;
using SunsetSystems.Party;
using SunsetSystems.UI.Utils;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SunsetSystems.UI
{
    public class InventoryItemDisplay : Button, IUserInterfaceView<InventoryEntry>
    {
        [SerializeField, ReadOnly]
        private InventoryEntry _itemEntry;
        [SerializeField]
        private Image _icon;
        [SerializeField]
        private TextMeshProUGUI _stackSize;

        private AssetReferenceSprite lastLoadedSprite;

        public async void UpdateView(IGameDataProvider<InventoryEntry> dataProvider)
        {
            ResetView();
            //if (lastLoadedSprite != null)
                //AddressableManager.Instance.ReleaseAsset(lastLoadedSprite);
            if (dataProvider != null)
            {
                _itemEntry = dataProvider.Data;
                lastLoadedSprite = _itemEntry._item.Icon;
                _icon.sprite = await AddressableManager.Instance.LoadAssetAsync(lastLoadedSprite);
                _icon.gameObject.SetActive(true);
                if (_itemEntry._item.Stackable)
                {
                    _stackSize.gameObject.SetActive(true);
                    _stackSize.text = _itemEntry._stackSize.ToString();
                }
                else
                {
                    _stackSize.gameObject.SetActive(false);
                }
                interactable = true;
            }
        }

        public void OnClick()
        {
            //TODO: Handle equip item in double click or right click context menu
            Debug.Log($"Equipping item {_itemEntry._item.Name}!");
            if (_itemEntry._item is EquipableItem item)
            {
                throw new NotImplementedException();
                //if (!InventoryManager.TryEquipItem(CharacterSelector.SelectedCharacterKey, item))
                //    Debug.LogError($"Failed to equip item {item.ReadableID} for character {CharacterSelector.SelectedCharacterKey}!");
            }
        }

        private void ResetView()
        {
            _icon.gameObject.SetActive(false);
            //_itemEntry = null;
            _stackSize.gameObject.SetActive(false);
            interactable = false;
        }

        public void OpentContextMenu()
        {
            //TODO: Actually make a context menu for an item instead of just equipping it
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            if (eventData.button.Equals(PointerEventData.InputButton.Right) && interactable)
            {
                OpentContextMenu();
            }
        }
    }
}
