using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using SunsetSystems.Inventory;
using UnityEngine;

namespace SunsetSystems.Economy
{
    [Serializable]
    public class CashTradeOffer : ITradeOffer
    {
        [SerializeField]
        private TradeOfferType _offerType;
        [SerializeField, Min(0), VerticalGroup("Price Settings"), LabelWidth(50)]
        private float _unitCost = 10;
        [SerializeField, Min(0), VerticalGroup("Price Settings"), LabelWidth(50)]
        private int _unitAmount = 1;
        [OdinSerialize]
        private ITradeable _offerItem;

        private float _unitCostMultiplier = 1f;

        public CashTradeOffer(ITradeable offerItem, TradeOfferType offerType)
        {
            _offerItem = offerItem;
            _offerType = offerType;
            _unitCost = offerItem.GetBaseValue();
            _unitAmount = 1;
        }

        public bool AcceptOffer()
        {
            return _offerType switch
            {
                TradeOfferType.Buy => _offerItem.Buy(_unitAmount),
                TradeOfferType.Sell => _offerItem.Sell(_unitAmount),
                _ => false,
            };
        }

        public float GetCost()
        {
            return _unitCost * _unitCostMultiplier * _unitAmount;
        }

        public bool CanPlayerAffordOffer()
        {
            throw new NotImplementedException();
        }

        public void SetCostMultiplier(float multiplier)
        {
            _unitCostMultiplier = multiplier;
        }

        public TradeOfferType GetOfferType() => _offerType;
    }
}
