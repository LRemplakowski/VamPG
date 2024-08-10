using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Inventory.Data
{
    [CreateAssetMenu(fileName = "New Consumable", menuName = "Sunset Inventory/Items/Consumable")]
    public class Consumable : BaseItem
    {
        [field: SerializeField]
        public List<IScriptableItem> Scripts { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            ItemCategory = ItemCategory.CONSUMABLE;
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            ItemCategory = ItemCategory.CONSUMABLE;
        }
    }
}
