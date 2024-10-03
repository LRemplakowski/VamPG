using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace SunsetSystems.Economy
{
    public class Merchant : SerializedMonoBehaviour, IMerchant
    {
        [BoxGroup("Merchant Config")]
        [SerializeField]
        private Currency _supportedCurrencies;
        [BoxGroup("Merchant Config")]
        [SerializeField]
        private float _buyFromPlayerMarkup = 1f, _sellToPlayerMarkup = 1f;
        [BoxGroup("Merchant Config")]
        [SerializeField]
        private bool _buysOnlySpecificTradeables = false;
        [PropertySpace]
        [BoxGroup("Merchant Config")]
        [SerializeField, HideIf("@this._buysOnlySpecificTradeables")]
        private TradeCategory _boughtTradeCategories;
        [PropertySpace]
        [BoxGroup("Merchant Config")]
        [SerializeField, ShowIf("@this._buysOnlySpecificTradeables")]
        private List<ITradeable> _acceptedTradeables = new();

        private bool SupportsCash => _supportedCurrencies.HasFlag(Currency.Money);
        private bool SupportsBoon => _supportedCurrencies.HasFlag(Currency.Boon);

        [TabGroup("Cash Offers")]
        [TableList(AlwaysExpanded = true, HideToolbar = true)]
        [PropertySpace]
        [Title("Trade Offer List")]
        [OdinSerialize, ShowIf("SupportsCash"), LabelText("Offer List"), PropertyOrder(1)]
        private List<CashTradeOffer> _cashOffers = new();

        [TabGroup("Boon Offers")]
        [TableList(AlwaysExpanded = true, HideToolbar = true)]
        [PropertySpace]
        [Title("Trade Offer List")]
        [OdinSerialize, ShowIf("SupportsBoon"), LabelText("Offer List"), PropertyOrder(1)]
        private List<BoonTradeOffer> _boonOffers = new();

        [TabGroup("Cash Offers")]
        [Button(Expanded = true), ShowIf("SupportsCash"), PropertyOrder(0)]
        public void AddCashOffer(ITradeable offeredItem)
        {
            _cashOffers.Add(new(offeredItem, TradeOfferType.SellToPlayer, this));
        }

        [TabGroup("Boon Offers")]
        [Button(Expanded = true), ShowIf("SupportsBoon"), PropertyOrder(0)]
        public void AddBoonOffer(ITradeable offeredItem)
        {
            _boonOffers.Add(new());
        }

        private void OnValidate()
        {
            _cashOffers ??= new();
            _boonOffers ??= new();
        }

        private void Start()
        {
            ITradeOffer.OnTradeOfferAccepted += OnOfferAccepted;
        }

        private void OnDestroy()
        {
            ITradeOffer.OnTradeOfferAccepted -= OnOfferAccepted;
        }

        public bool GetAvailableTradeOffers(TradeOfferType offerType, Currency currencyType, out IEnumerable<AbstractTradeOffer> tradeOffers)
        {
            tradeOffers = default;
            if (_supportedCurrencies.HasFlag(currencyType) is false)
                return false;
            return false;
        }

        public Currency GetSupportedCurrencies()
        {
            return _supportedCurrencies;
        }

        public float GetMerchantSellMarkup()
        {
            return _sellToPlayerMarkup;
        }

        public float GetMerchantBuyMarkup()
        {
            return _buyFromPlayerMarkup;
        }

        public void OnOfferAccepted(ITradeOffer offer)
        {
            if (offer.IsOfferedByMerchant(this))
            {

            }
        }
    }
}
