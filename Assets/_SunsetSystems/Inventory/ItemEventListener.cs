using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using SunsetSystems.Inventory.Data;
using UltEvents;
using UnityEngine;

namespace SunsetSystems.Inventory
{
    [DisallowMultipleComponent]
    public class ItemEventListener : SerializedMonoBehaviour
    {
        [Title("Config")]
        [SerializeField]
        private bool _useItemList = false;
        [SerializeField, HideIf("@this._useItemList == true")]
        private IBaseItem _eventItem;
        [SerializeField, ShowIf("@this._useItemList == true")]
        private List<IBaseItem> _eventItems = new();

        [Title("Events")]
        public UltEvent OnItemAcquired;
        public UltEvent OnItemLost;

        public string ComponentID => $"ITEM_EVENT_LISTENER";

        private void OnEnable()
        {
            InventoryManager.OnItemAcquired += ItemGained;
            InventoryManager.OnItemLost += ItemLost;
        }

        private void OnDisable()
        {
            InventoryManager.OnItemAcquired -= ItemGained;
            InventoryManager.OnItemLost -= ItemLost;
        }

        private void ItemLost(IBaseItem item)
        {
            if (ValidateItem(item))
                OnItemLost?.InvokeSafe();
        }

        private void ItemGained(IBaseItem item)
        {
            if (ValidateItem(item))
                OnItemAcquired?.InvokeSafe();
        }

        private bool ValidateItem(IBaseItem item)
        {
            if (item == null)
                return false;
            if (_useItemList)
                return _eventItems.Any(it => it.DatabaseID == item.DatabaseID);
            else
                return _eventItem.DatabaseID == item.DatabaseID;
        }
    }
}
