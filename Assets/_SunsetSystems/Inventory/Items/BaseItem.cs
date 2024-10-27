using Sirenix.OdinInspector;
using SunsetSystems.Core.Database;
using SunsetSystems.Economy;
using SunsetSystems.Journal;
using SunsetSystems.UI.Utils;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SunsetSystems.Inventory.Data
{
    public abstract class BaseItem : AbstractDatabaseEntry<IBaseItem>, IRewardable, IUserInfertaceDataProvider<IBaseItem>, IBaseItem, ITradeable
    {
        [field: SerializeField]
        public string Name { get; protected set; }
        [field: SerializeField]
        public override string ReadableID { get; protected set; }
        [field: SerializeField, ReadOnly]
        public override string DatabaseID { get; protected set; }
        [field: SerializeField, ReadOnly]
        public ItemCategory ItemCategory { get; protected set; }
        [SerializeField, Min(0)]
        private float _itemCashValue;
        [field: SerializeField, TextArea]
        public string ItemDescription { get; protected set; }
        [field: SerializeField]
        public AssetReferenceGameObject WorldSpaceRepresentation { get; protected set; }
        [field: SerializeField]
        public Sprite Icon { get; protected set; }
        [field: SerializeField]
        public bool Stackable { get; protected set; }

        public IBaseItem UIData => this;

        public void ApplyReward(int amount)
        {
            InventoryManager.Instance.GiveItemToPlayer(this, amount);
        }

        public bool HandlePlayerBought(int amount)
        {
            InventoryManager.Instance.GiveItemToPlayer(this, amount);
            return true;
        }

        public bool HandlePlayerSold(int amount)
        {
            return InventoryManager.Instance.TakeItemFromPlayer(this, amount, false);
        }

        public float GetBaseValue()
        {
            return _itemCashValue;
        }

        public TradeCategory GetTradeCategory()
        {
            return TradeCategory.Item;
        }

        public string GetTradeName() => Name;

        public Sprite GetTradeIcon() => Icon;

        protected override void RegisterToDatabase()
        {
            ItemDatabase.Instance?.Register(this);
        }

        protected override void UnregisterFromDatabase()
        {
            ItemDatabase.Instance?.Unregister(this);
        }
    }
}
