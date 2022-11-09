using NaughtyAttributes;
using SunsetSystems.Inventory;
using SunsetSystems.Inventory.Data;
using SunsetSystems.UI.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SunsetSystems.UI
{
    public class InventoryItemDisplay : MonoBehaviour, IUserInterfaceView<InventoryEntry, InventoryItemDisplay>
    {
        [SerializeField, ReadOnly]
        private InventoryEntry _itemEntry;
        [SerializeField]
        private Image _icon;
        [SerializeField]
        private TextMeshProUGUI _stackSize;


        private void OnEnable()
        {
            ResetView();
        }

        public void UpdateView(IGameDataProvider<InventoryEntry> dataProvider)
        {
            ResetView();
            if (dataProvider != null)
            {
                _itemEntry = dataProvider.Data;
                _icon.sprite = _itemEntry._item.Icon;
                _icon.gameObject.SetActive(true);
                if (_itemEntry._stackSize > 0)
                {
                    _stackSize.gameObject.SetActive(true);
                    _stackSize.text = _itemEntry._stackSize.ToString();
                }
                else
                {
                    _stackSize.gameObject.SetActive(false);
                }
            }
        }

        private void OnClick()
        {
            //TODO: Handle equip item in double click or right click context menu
            Debug.Log($"Equipping item {_itemEntry._item.ItemName}!");
        }

        private void ResetView()
        {
            _icon.gameObject.SetActive(false);
            _itemEntry = null;
        }
    }
}
