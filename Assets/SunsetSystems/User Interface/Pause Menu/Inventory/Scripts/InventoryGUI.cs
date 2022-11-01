using SunsetSystems.Inventory;
using SunsetSystems.Inventory.Data;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SunsetSystems.UserInterface
{
    public class InventoryGUI : MonoBehaviour
    {
        [SerializeField]
        private InventoryItemDisplay _displayPrefab;
        [SerializeField]
        private GameObject _itemListContentParent;

        public void AddItems(List<BaseItem> items)
        {
            items?.ForEach(item => AddItem(item));
        }

        public void AddItem(BaseItem item)
        {
            if (item == null)
                return;
            InventoryItemDisplay itemDisplay = Instantiate(_displayPrefab, _itemListContentParent.transform);
            itemDisplay.item = item;
        }

        public void ClearItemList()
        {
            _itemListContentParent.transform.DestroyChildren();
        }

        private void OnEnable()
        {
            AddItems(InventoryManager.PlayerInventory.Contents.Select(entry => entry._item).ToList());
        }

        private void OnDisable()
        {
            ClearItemList();
        }
    }
}
