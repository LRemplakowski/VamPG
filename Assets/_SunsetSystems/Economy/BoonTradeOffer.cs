using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using SunsetSystems.Economy.UI;
using UnityEngine;

namespace SunsetSystems.Economy
{
    [Serializable]
    public class BoonTradeOffer : AbstractTradeOffer
    {
        [SerializeField, ReadOnly]
        private TradeOfferType _offerType;

        public int RequiredBoonScore;
        public string RequiredBoonSource;
        public bool RequiresSpecificBoon;
        [OdinSerialize, ReadOnly]
        private ITradeable _offeredItem;

        public BoonTradeOffer() : base(null)
        {

        }

        public BoonTradeOffer(ITradeable offeredItem, TradeOfferType offerType, IMerchant merchant) : base(merchant)
        {
            _offeredItem = offeredItem;
            _offerType = offerType;
        }

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
