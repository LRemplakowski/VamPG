using NaughtyAttributes;
using SunsetSystems.Inventory.Data;
using SunsetSystems.UI.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SunsetSystems.UserInterface
{
    public class InventoryItemDisplay : MonoBehaviour, IUserInterfaceView<BaseItem, InventoryItemDisplay>
    {
        [SerializeField, ReadOnly]
        private BaseItem _item;
        [SerializeField, ReadOnly]
        private Image _icon;


        public void UpdateView(IGameDataProvider<BaseItem> dataProvider)
        {
            ResetView();
            if (dataProvider != null)
            {
                _item = dataProvider.Data;
                _icon.sprite = _item.Icon;
            }
        }

        private void ResetView()
        {
            _icon.sprite = null;
            _item = null;
        }
    }
}
