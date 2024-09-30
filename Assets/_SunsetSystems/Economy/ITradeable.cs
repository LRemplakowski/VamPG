using UnityEngine;

namespace SunsetSystems.Economy
{
    public interface ITradeable
    {
        bool HandlePlayerBought(int amount);
        bool HandlePlayerSold(int amount);
        float GetBaseValue();
        TradeCategory GetTradeCategory();
        string GetTradeName();
        Sprite GetTradeIcon();
    }
}
