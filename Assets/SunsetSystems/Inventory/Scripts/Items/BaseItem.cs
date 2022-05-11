using SunsetSystems.Resources;
using UnityEditor;
using UnityEngine;

namespace SunsetSystems.Inventory.Data
{
    public abstract class BaseItem : ScriptableObject
    {
        [SerializeField]
        protected string _itemName;
        public string ItemName { get => _itemName; }
        [field: SerializeField, ReadOnly]
        public string ID { get; private set; }
        [SerializeField, ReadOnly]
        protected ItemCategory _itemCategory;
        public ItemCategory ItemCategory => _itemCategory;
        [SerializeField, TextArea]
        protected string _itemDescription;
        public string ItemDescription { get => _itemDescription; }
        [SerializeField]
        protected GameObject _prefab;
        public GameObject Prefab { get => _prefab; }
        [SerializeField]
        protected Sprite _icon;
        public Sprite Icon { get => _icon; }
        [field: SerializeField]
        public bool Stackable { get; private set; }

        private void OnValidate()
        {
#if UNITY_EDITOR
            if (_itemName == "")
            {
                _itemName = name;
                EditorUtility.SetDirty(this);
            }
            if (_icon == null)
            {
                _icon = ResourceLoader.GetFallbackIcon();
                EditorUtility.SetDirty(this);
            }
            if (ID == "")
            {
                ID = GUID.Generate().ToString();
                EditorUtility.SetDirty(this);
            }
#endif
        }
    }
}
