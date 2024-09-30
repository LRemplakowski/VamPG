using System;
using System.Collections;
using UnityEngine;

namespace SunsetSystems.Economy.UI
{
    public interface ITradeOfferViewData
    {
        string Title { get; }
        Sprite Icon { get; }
        int UnitAmount { get; }
        float OfferPrice { get; }

        Func<bool> OfferAcceptanceDelegate { get; }

        bool CanPlayerAffordOffer();
    }
}