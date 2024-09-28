using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Economy
{
    public interface IMerchantConfig
    {
        public Currency GetSupportedCurrencies();
        public bool GetAvailableTradeOffers(Currency currencyType, out IEnumerable<ITradeOffer> tradeOffers);
    }
}
