using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Inventory.Data
{
    [CreateAssetMenu(fileName = "New Trousers", menuName = "Sunset Inventory/Items/Trousers")]
    public class Trousers : WearableItem
    {
        protected override void Awake()
        {
            base.Awake();
            ItemCategory = ItemCategory.TROUSERS;
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            ItemCategory = ItemCategory.TROUSERS;
        }
    }
}
