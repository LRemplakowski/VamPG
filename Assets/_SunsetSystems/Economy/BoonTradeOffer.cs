using System.Collections;
using System.Collections.Generic;
using SunsetSystems.Economy;
using UnityEngine;

namespace SunsetSystems
{
    public class BoonTradeOffer : ITradeOffer
    {
        public int RequiredBoonScore;
        public string RequiredBoonSource;
        public bool RequiresSpecificBoon;

        public bool AcceptOffer()
        {
            throw new System.NotImplementedException();
        }

        public bool CanPlayerAffordOffer()
        {
            throw new System.NotImplementedException();
        }

        public float GetCost()
        {
            throw new System.NotImplementedException();
        }
    }
}
