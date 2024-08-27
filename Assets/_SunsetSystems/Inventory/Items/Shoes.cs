using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Inventory.Data
{
    [CreateAssetMenu(fileName = "New Shoes", menuName = "Sunset Inventory/Items/Shoes")]
    public class Shoes : WearableItem
    {
        protected override void Awake()
        {
            base.Awake();
            ItemCategory = ItemCategory.SHOES;
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            ItemCategory = ItemCategory.SHOES;
        }
    }
}
