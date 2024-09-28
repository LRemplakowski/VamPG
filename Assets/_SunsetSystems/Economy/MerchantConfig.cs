using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace SunsetSystems.Economy
{
    [CreateAssetMenu(fileName = "New Merchant Config", menuName = "Sunset Economy/Merchant Config")]
    public class MerchantConfig : SerializedMonoBehaviour, IMerchantConfig
    {
        [SerializeField]
        private Currency _supportedCurrencies;
        [OdinSerialize, TabGroup("Cash Config"), ShowIf("@this._supportedCurrencies.HasFlag(Currency.Money)"), InlineProperty, HideLabel, HideReferenceObjectPicker]
        private CurrencyOffersConfig _cashConfig = new();
        [OdinSerialize, TabGroup("Boon Config"), ShowIf("@this._supportedCurrencies.HasFlag(Currency.Boon)"), InlineProperty, HideLabel, HideReferenceObjectPicker]
        private CurrencyOffersConfig _boonConfig = new();

        private void OnValidate()
        {
            //_cashConfig.TradeOffers = SortOffers(_cashConfig.TradeOffers);
            //_boonConfig.TradeOffers = SortOffers(_boonConfig.TradeOffers);
        }

        private List<CashTradeOffer> SortOffers(List<CashTradeOffer> offersToSort)
        {
            return offersToSort.OrderBy(offer => offer.GetOfferType()).ToList();
        }

        private void Start()
        {
            InjectTradeOfferCostMultipliers(_cashConfig);
            InjectTradeOfferCostMultipliers(_boonConfig);
        }

        private void InjectTradeOfferCostMultipliers(CurrencyOffersConfig currencyConfig)
        {
            //foreach (var tradeOffer in currencyConfig.TradeOffers)
            //{
            //    switch (tradeOffer.GetOfferType())
            //    {
            //        case TradeOfferType.Buy:
            //            tradeOffer.SetCostMultiplier(currencyConfig.BuyMultiplier);
            //            break;
            //        case TradeOfferType.Sell:
            //            tradeOffer.SetCostMultiplier(currencyConfig.SellMultiplier);
            //            break;
            //    }
            //}
        }

        public bool GetAvailableTradeOffers(Currency currencyType, out IEnumerable<ITradeOffer> tradeOffers)
        {
            tradeOffers = default;
            if (_supportedCurrencies.HasFlag(currencyType) is false)
                return false;
            switch (currencyType)
            {
                case Currency.Money:
                    tradeOffers = _cashConfig.TradeOffers;
                    return true;
                case Currency.Boon:
                    tradeOffers = _boonConfig.TradeOffers;
                    return true;
                default:
                    return false;
            }
        }

        public Currency GetSupportedCurrencies()
        {
            return _supportedCurrencies;
        }

        [Serializable]
        private class CurrencyOffersConfig
        {
            [SerializeField]
            public float BuyMultiplier = 1f;
            [SerializeField]
            public float SellMultiplier = 1f;
            [OdinSerialize, TableList]
            public List<CashTradeOffer> TradeOffers = new();
        }
    }
}
