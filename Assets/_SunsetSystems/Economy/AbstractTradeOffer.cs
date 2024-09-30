using System.Collections;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using SunsetSystems.Economy.UI;
using UnityEngine;

namespace SunsetSystems.Economy
{
    public abstract class AbstractTradeOffer : ITradeOffer
    {
        public abstract bool AcceptOffer();

        public abstract bool CanPlayerAffordOffer();

        public abstract float GetCost();

        public abstract ITradeOfferViewData GetOfferViewData();
    }
}