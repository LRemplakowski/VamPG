using Sirenix.OdinInspector;
using SunsetSystems.Economy.UI;
using UnityEngine;

namespace SunsetSystems.Economy
{
    public abstract class AbstractTradeOffer : ITradeOffer
    {
        [field: SerializeField, HideInTables]
        protected IMerchant OwnerConfig { get; private set; }

        public AbstractTradeOffer(IMerchant merchant)
        {
            OwnerConfig = merchant;
        }

        public abstract bool AcceptOffer();

        public abstract bool CanPlayerAffordOffer();

        public abstract float GetCost();

        public abstract ITradeOfferViewData GetOfferViewData();

        public bool IsOfferedByMerchant(IMerchant merchant)
        {
            return OwnerConfig != null && OwnerConfig.Equals(merchant);
        }
    }
}