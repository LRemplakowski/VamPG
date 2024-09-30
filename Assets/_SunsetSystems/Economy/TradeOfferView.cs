using Sirenix.OdinInspector;
using SunsetSystems.Utils.ObjectPooling;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SunsetSystems.Economy.UI
{
    public class TradeOfferView : SerializedMonoBehaviour, ITradeOfferView, IPooledObject
    {
        [SerializeField]
        private Selectable _offerSelectable;
        [SerializeField]
        private Image _offerIcon;
        [SerializeField]
        private TextMeshProUGUI _offerTitle;
        [SerializeField]
        private TextMeshProUGUI _offerCost;

        private ITradeOfferViewData _cachedViewData;
        private TradingInterface _tradeUIManager;

        public void SetupView(ITradeOfferViewData viewData, TradingInterface tradeUIManager)
        {
            _cachedViewData = viewData;
            _tradeUIManager = tradeUIManager;
            RefreshView();
        }

        public void OnOfferClicked()
        {
            _tradeUIManager.ShowTradeConfirmation(_cachedViewData.OfferAcceptanceDelegate, _cachedViewData);
        }

        public void RefreshView()
        {
            _offerIcon.sprite = _cachedViewData.Icon;
            _offerTitle.text = _cachedViewData.UnitAmount > 1 ? $"{_cachedViewData.Title} ({_cachedViewData.UnitAmount})" : _cachedViewData.Title;
            _offerCost.text = $"{_cachedViewData.OfferPrice:0.00} $";
            _offerSelectable.interactable = _cachedViewData.CanPlayerAffordOffer();
        }

        public void ResetObject()
        {
            _offerIcon.sprite = null;
            _offerTitle.text = "";
            _offerCost.text = "0";
            _offerSelectable.interactable = false;
        }
    }
}
