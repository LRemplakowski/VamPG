using System;
using SunsetSystems.Economy;
using SunsetSystems.Economy.UI;

namespace SunsetSystems
{
    [Serializable]
    public class BoonTradeOffer : AbstractTradeOffer
    {
        public int RequiredBoonScore;
        public string RequiredBoonSource;
        public bool RequiresSpecificBoon;

        public override bool AcceptOffer()
        {
            throw new System.NotImplementedException();
        }

        public override bool CanPlayerAffordOffer()
        {
            throw new System.NotImplementedException();
        }

        public override float GetCost()
        {
            throw new System.NotImplementedException();
        }

        public override ITradeOfferViewData GetOfferViewData()
        {
            throw new NotImplementedException();
        }
    }
}
