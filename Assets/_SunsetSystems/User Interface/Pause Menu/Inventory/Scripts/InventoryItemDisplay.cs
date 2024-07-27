using Sirenix.OdinInspector;
using SunsetSystems.Inventory;
using SunsetSystems.Inventory.Data;
using SunsetSystems.Party;
using SunsetSystems.UI.Utils;
using TMPro;
using UnityEngine;
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

        public void UpdateView(IGameDataProvider<InventoryEntry> dataProvider)
        {
            ResetView();
            //if (lastLoadedSprite != null)
                //AddressableManager.Instance.ReleaseAsset(lastLoadedSprite);
            if (dataProvider != null)
            {
                _itemEntry = dataProvider.Data;
                _icon.sprite = _itemEntry.ItemReference.Icon;
                _icon.gameObject.SetActive(true);
                if (_itemEntry.ItemReference.Stackable)
                {
                    _stackSize.gameObject.SetActive(true);
                    _stackSize.text = _itemEntry.StackSize.ToString();
                }
                else
                {
                    _stackSize.gameObject.SetActive(false);
                }
                interactable = true;
            }
            else
            {
                _icon.gameObject.SetActive(false);
                _stackSize.gameObject.SetActive(false);
                interactable = false;
            }
        }

        public void OnClick()
        {
            if (_itemEntry.ItemReference is IEquipableItem equipableItem)
            {
                if (InventoryManager.Instance.TakeItemFromPlayer(equipableItem, postLogMessage: false) is false)
                    return;
                string currentCharacterID = CharacterSelector.SelectedCharacterKey;
                var eqManager = PartyManager.Instance.GetPartyMemberByID(currentCharacterID).References.EquipmentManager;
                var slotID = eqManager.GetSlotForItem(equipableItem);
                if (eqManager.EquipItem(slotID, equipableItem, out var previouslyEquipped))
                {
                    if (ShouldAddItemBackToInventory(previouslyEquipped))
                        InventoryManager.Instance.GiveItemToPlayer(previouslyEquipped, postLogMessage: false);
                }
                else
                {
                    InventoryManager.Instance.GiveItemToPlayer(equipableItem, postLogMessage: false);
                }
            }
        }

        private bool ShouldAddItemBackToInventory(IEquipableItem item)
        {
            return item != null && !item.IsDefaultItem;
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

        //public override void OnPointerClick(PointerEventData eventData)
        //{
        //    base.OnPointerClick(eventData);
        //    if (eventData.button.Equals(PointerEventData.InputButton.Right) && interactable)
        //    {
        //        OpentContextMenu();
        //    }
        //}
    }
}
