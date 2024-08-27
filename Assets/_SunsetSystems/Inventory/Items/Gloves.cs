using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Inventory.Data
{
    [CreateAssetMenu(fileName = "New Gloves", menuName = "Sunset Inventory/Items/Gloves")]
    public class Gloves : WearableItem
    {
        protected override void Awake()
        {
            base.Awake();
            ItemCategory = ItemCategory.GLOVES;
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            ItemCategory = ItemCategory.GLOVES;
        }
    }
}
