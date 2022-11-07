using SunsetSystems.Inventory;
using SunsetSystems.Inventory.Data;
using SunsetSystems.UI.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SunsetSystems.UserInterface
{
    public class InventoryUIParent : MonoBehaviour, IUserInterfaceUpdateReciever<BaseItem, InventoryItemDisplay>
    {
        [SerializeField]
        private GameObject _itemListContentParent;

        [SerializeField]
        private InventoryItemDisplay _displayPrefab;
        public InventoryItemDisplay ViewPrefab => _displayPrefab;
        [SerializeField]
        private Transform _viewParent;
        public Transform ViewParent => _viewParent;

        public List<IUserInterfaceView<BaseItem, InventoryItemDisplay>> ViewPool => throw new System.NotImplementedException();

        private void OnEnable()
        {
            
        }

        public void DisableViews()
        {
            throw new System.NotImplementedException();
        }
    }
}
