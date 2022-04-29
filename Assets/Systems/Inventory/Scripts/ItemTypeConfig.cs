using SunsetSystems.Inventory.Data;
using UnityEngine;

namespace SunsetSystems.Inventory
{
    [CreateAssetMenu(fileName = "Item Types Config", menuName = "Inventory/Item Type Config")]
    public class ItemTypeConfig : ScriptableObject
    {
        [SerializeField]
        private ItemTypeTemplate[] _itemTemplates;

        private void OnValidate()
        {
            foreach (ItemTypeTemplate template in _itemTemplates)
            {
                if (template.Name.Length == 0)
                    template.SetName("Item type");
            }
        }
    }

    [System.Serializable]
    public class ItemTypeTemplate
    {
        [SerializeField]
        private string _name;
        public string Name { get => _name; }
        [SerializeField]
        private ItemAttribute[] _itemTypeAttributes;

        internal void SetName(string _name)
        {
            this._name = _name;
        }
    }
}
