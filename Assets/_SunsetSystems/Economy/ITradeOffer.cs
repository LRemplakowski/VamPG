using System;
using SunsetSystems.Economy.UI;

namespace SunsetSystems.Economy
{
    public interface ITradeOffer
    {
        public static Action<ITradeOffer> OnTradeOfferAccepted;

        float GetCost();
        bool AcceptOffer();
        bool CanPlayerAffordOffer();
        ITradeOfferViewData GetOfferViewData();
        bool IsOfferedByMerchant(IMerchant merchant);
    }
}