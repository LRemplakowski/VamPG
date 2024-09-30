using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SunsetSystems.Utils.ObjectPooling;
using UnityEngine;

namespace SunsetSystems.Economy.UI
{
    public class TradingInterface : SerializedMonoBehaviour
    {
        [SerializeField]
        private IObjectPool<TradeOfferView> _offerViewPool;
        [SerializeField]
        private TradeConfirmationWindow _tradeConfirmationWindow;

        private List<TradeOfferView> _tradeOfferViews = new();

        private Func<bool> _cachedTradeConfirmDelegate;

        public void InitializeTradeInterface(IMerchant merchant)
        {
            _tradeOfferViews.ForEach(view => _offerViewPool.ReturnObject(view));
            _tradeOfferViews.Clear();
            merchant.GetAvailableTradeOffers(TradeOfferType.Buy, merchant.GetSupportedCurrencies(), out var offers);
            foreach (var offer in offers)
            {
                var offerView = _offerViewPool.GetPooledObject();
                offerView.SetupView(offer.GetOfferViewData(), this);
                _tradeOfferViews.Add(offerView);
            }
            gameObject.SetActive(true);
        }

        public void ShowTradeConfirmation(Func<bool> onTradeConfirmedDelegate, ITradeOfferViewData viewData)
        {
            _cachedTradeConfirmDelegate = onTradeConfirmedDelegate;
            _tradeConfirmationWindow.UpdateTradeConfirmationText(viewData);
            _tradeConfirmationWindow.SetConfirmationWindowVisible(true);
        }

        public void OnTradeConfirmed()
        {
            if (_cachedTradeConfirmDelegate?.Invoke() ?? false)
            {
                _tradeConfirmationWindow.SetConfirmationWindowVisible(false);
                _tradeConfirmationWindow = null;
                _tradeOfferViews.ForEach(view => view.RefreshView());
            }
        }

        public void OnTradeCanceled()
        {
            _cachedTradeConfirmDelegate = null;
            _tradeConfirmationWindow.SetConfirmationWindowVisible(false);
        }
    }
}