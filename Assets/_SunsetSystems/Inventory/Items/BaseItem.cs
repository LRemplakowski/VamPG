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
        [field: SerializeField, BoxGroup("Base Item")]
        public string Name { get; protected set; }
        [field: SerializeField, BoxGroup("Base Item")]
        public override string ReadableID { get; protected set; }
        [field: SerializeField, ReadOnly, BoxGroup("Base Item")]
        public override string DatabaseID { get; protected set; }
        [field: SerializeField, ReadOnly, BoxGroup("Base Item")]
        public ItemCategory ItemCategory { get; protected set; }
        [SerializeField, MinValue(0), BoxGroup("Base Item")]
        private float _itemCashValue;
        [field: SerializeField, MultiLineProperty, BoxGroup("Base Item")]
        public string ItemDescription { get; protected set; }
        [field: SerializeField, BoxGroup("Base Item")]
        public AssetReferenceGameObject WorldSpaceRepresentation { get; protected set; }
        [field: SerializeField, BoxGroup("Base Item")]
        public Sprite Icon { get; protected set; }
        [field: SerializeField, BoxGroup("Base Item")]
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
