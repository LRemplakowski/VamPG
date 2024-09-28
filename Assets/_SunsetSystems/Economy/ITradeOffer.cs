using System.Collections;
using UnityEngine;

namespace SunsetSystems.Economy
{
    public interface ITradeOffer
    {
        public float GetCost();
        public bool AcceptOffer();
        public bool CanPlayerAffordOffer();
    }
}