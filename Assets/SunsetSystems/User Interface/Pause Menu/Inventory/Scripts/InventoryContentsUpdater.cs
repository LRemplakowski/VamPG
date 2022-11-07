using SunsetSystems.Inventory.Data;
using SunsetSystems.UI.Utils;
using SunsetSystems.UserInterface;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SunsetSystems.Inventory.UI
{
    public class InventoryContentsUpdater : MonoBehaviour, IUserInterfaceUpdateReciever<BaseItem, InventoryItemDisplay>
    {
        [SerializeField]
        private InventoryItemDisplay _viewPrefab;
        public InventoryItemDisplay ViewPrefab => _viewPrefab;
        [SerializeField]
        private int _minimumVisibleRows;
        [SerializeField]
        private GridLayoutGroup _contentGrid;
        private int ColumnConstraint => _contentGrid.constraintCount;

        public Transform ViewParent => transform;

        private readonly List<IUserInterfaceView<BaseItem, InventoryItemDisplay>> _viewPool = new();
        public List<IUserInterfaceView<BaseItem, InventoryItemDisplay>> ViewPool => _viewPool;

        private void Start()
        {
            ViewPool.AddRange(GetComponentsInChildren<InventoryItemDisplay>());
            for (int i = ViewPool.Count; i < ColumnConstraint * _minimumVisibleRows; i++)
            {
                InventoryItemDisplay display = Instantiate(_viewPrefab, ViewParent);
                ViewPool.Add(display);
            }
            DisableViews();
        }

        private void OnDisable()
        {
            DisableViews();
        }

        public void DisableViews()
        {
            for (int i = 0; i < ViewPool.Count; i++)
            {
                IUserInterfaceView<BaseItem, InventoryItemDisplay> itemDisplay = ViewPool[i];
                itemDisplay.UpdateView(null);
                if (i < _minimumVisibleRows * ColumnConstraint)
                {
                    (itemDisplay as MonoBehaviour).gameObject.SetActive(true);
                    continue;
                }
                (itemDisplay as MonoBehaviour).gameObject.SetActive(false);
            }
        }
    }
}
