using SunsetSystems.Economy.UI;

namespace SunsetSystems.Economy
{
    public interface ITradeOffer
    {
        float GetCost();
        bool AcceptOffer();
        bool CanPlayerAffordOffer();
        ITradeOfferViewData GetOfferViewData();
    }
}