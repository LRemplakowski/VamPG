using UnityEngine;

namespace SunsetSystems.Inventory.Data
{
    public abstract class BaseItem : ScriptableObject
    {
        [SerializeField]
        protected string _itemName;
        public string ItemName { get => _itemName; }
        [SerializeField, TextArea]
        protected string _itemDescription;
        public string ItemDescription { get => _itemDescription; }
        [SerializeField]
        protected GameObject _prefab;
        public GameObject Prefab { get => _prefab; }
        [SerializeField]
        protected Texture2D _icon;
        public Texture2D Icon { get => _icon; }
        [SerializeField, ReadOnly]
        protected ItemCategory _itemCategory;
        public ItemCategory ItemCategory => _itemCategory;
    }
}
