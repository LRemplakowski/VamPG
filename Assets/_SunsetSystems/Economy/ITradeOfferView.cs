namespace SunsetSystems.Economy.UI
{
    public interface ITradeOfferView
    {
        void SetupView(ITradeOfferViewData viewData, TradingInterface tradeUIManager);
        void RefreshView();
    }
}
