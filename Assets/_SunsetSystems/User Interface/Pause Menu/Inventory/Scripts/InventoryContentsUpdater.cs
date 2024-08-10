using SunsetSystems.Inventory.Data;
using SunsetSystems.UI.Utils;
using SunsetSystems.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SunsetSystems.Inventory.UI
{
    public class InventoryContentsUpdater : MonoBehaviour, IUserInterfaceUpdateReciever<InventoryEntry>
    {
        [SerializeField]
        private InventoryItemDisplay _viewPrefab;
        public InventoryItemDisplay ViewPrefab => _viewPrefab;
        [SerializeField]
        private int _minimumVisibleRows;
        [SerializeField]
        private GridLayoutGroup _contentGrid;
        [SerializeField]
        private TextMeshProUGUI _moneyText;
        private int ColumnConstraint => _contentGrid.constraintCount;

        public Transform ViewParent => _contentGrid.transform;

        private readonly List<InventoryItemDisplay> _viewPool = new();
        public List<InventoryItemDisplay> ViewPool => _viewPool;

        private bool initializedOnce = false;

        private void Initialize()
        {
            ViewPool.AddRange(GetComponentsInChildren<InventoryItemDisplay>());
            for (int i = ViewPool.Count; i < ColumnConstraint * _minimumVisibleRows; i++)
            {
                InventoryItemDisplay display = Instantiate(_viewPrefab, ViewParent);
                ViewPool.Add(display);
            }
            initializedOnce = true;
        }

        public void UpdateViews(List<IUserInfertaceDataProvider<InventoryEntry>> data)
        {
            if (!initializedOnce)
                Initialize();
            DisableViews();
            if (data == null || data.Count <= 0)
            {
                Debug.LogWarning("UI Update Reciever recieved an empty or null collection!");
                return;
            }
            ViewParent.gameObject.SetActive(true);
            UpdateMoneyCounter(Mathf.RoundToInt(InventoryManager.Instance.GetMoneyAmount()));
            for (int i = 0; i < data.Count; i++)
            {
                IUserInfertaceDataProvider<InventoryEntry> dataProvider = data[i];
                if (dataProvider == null)
                {
                    Debug.LogError("Null DataProvider while creating view!");
                    continue;
                }

                InventoryItemDisplay view;
                if (ViewPool.Count > i)
                {
                    view = ViewPool[i];
                }
                else
                {
                    view = Instantiate(ViewPrefab, ViewParent);
                    ViewPool.Add(view);
                }
                view.UpdateView(dataProvider);
                view.gameObject.SetActive(true);
            }
        }

        public void UpdateMoneyCounter(int money)
        {
            _moneyText.text = $"{money.ToString("$ 0", System.Globalization.CultureInfo.CurrentCulture)}$";
        }


        private void OnDisable()
        {
            DisableViews();
        }

        public void DisableViews()
        {
            for (int i = 0; i < ViewPool.Count; i++)
            {
                InventoryItemDisplay itemDisplay = ViewPool[i];
                itemDisplay.UpdateView(null);
                if (i < _minimumVisibleRows * ColumnConstraint)
                {
                    itemDisplay.gameObject.SetActive(true);
                    continue;
                }
                itemDisplay.gameObject.SetActive(false);
            }
        }
    }
}
