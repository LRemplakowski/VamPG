using SunsetSystems.Inventory.Data;
using SunsetSystems.UI.Utils;
using SunsetSystems.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SunsetSystems.Inventory.UI
{
    public class InventoryContentsUpdater : MonoBehaviour, IUserInterfaceUpdateReciever<InventoryEntry, InventoryItemDisplay>
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

        private readonly List<IUserInterfaceView<InventoryEntry, InventoryItemDisplay>> _viewPool = new();
        public List<IUserInterfaceView<InventoryEntry, InventoryItemDisplay>> ViewPool => _viewPool;

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

        public void UpdateViews(IList<IGameDataProvider<InventoryEntry>> data)
        {
            DisableViews();
            if (data == null || data.Count <= 0)
            {
                Debug.LogWarning("UI Update Reciever recieved an empty or null collection!");
                return;
            }
            ViewParent.gameObject.SetActive(true);
            for (int i = 0; i < data.Count; i++)
            {
                IGameDataProvider<InventoryEntry> dataProvider = data[i];
                if (dataProvider == null)
                {
                    Debug.LogError("Null DataProvider while creating view!");
                    continue;
                }

                IUserInterfaceView<InventoryEntry, InventoryItemDisplay> view;
                if (ViewPool.Count > i)
                {
                    Debug.Log("Getting view from pool!");
                    view = ViewPool[i];
                }
                else
                {
                    Debug.Log("Instantiating new view!");
                    view = Instantiate(ViewPrefab, ViewParent);
                    ViewPool.Add(view);
                }
                view.UpdateView(dataProvider);
                (view as MonoBehaviour).gameObject.SetActive(true);
            }
        }

        public void UpdateMoneyCounter(int money)
        {

        }


        private void OnDisable()
        {
            DisableViews();
        }

        public void DisableViews()
        {
            for (int i = 0; i < ViewPool.Count; i++)
            {
                IUserInterfaceView<InventoryEntry, InventoryItemDisplay> itemDisplay = ViewPool[i];
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
