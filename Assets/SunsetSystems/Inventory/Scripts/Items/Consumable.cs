using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Inventory.Data
{
    [CreateAssetMenu(fileName = "New Consumable", menuName = "Sunset Inventory/Items/Consumable")]
    public class Consumable : BaseItem
    {
        [SerializeField, RequireInterface(typeof(IScriptableItem))]
        private List<Object> _scripts = new();
        public List<IScriptableItem> Scripts { get; private set; }

        private void Awake()
        {
            _itemCategory = ItemCategory.CONSUMABLE;
        }

        private void OnValidate()
        {
            Scripts = new();
            _scripts.ForEach(script => Scripts.Add(script as IScriptableItem));
        }
    }
}
