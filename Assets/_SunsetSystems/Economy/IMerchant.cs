using System.Collections.Generic;

namespace SunsetSystems.Economy
{
    public interface IMerchant
    {
        public Currency GetSupportedCurrencies();
        public bool FinalizeOffer(AbstractTradeOffer offer);
        public bool GetAvailableTradeOffers(TradeOfferType offerType, Currency currencyType, out IEnumerable<AbstractTradeOffer> tradeOffers);

        public float GetMerchantSellMarkup();
        public float GetMerchantBuyMarkup();
    }
}
