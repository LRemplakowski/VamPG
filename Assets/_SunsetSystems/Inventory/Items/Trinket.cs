using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Inventory.Data
{
    [CreateAssetMenu(fileName = "New Trinket", menuName = "Sunset Inventory/Items/Trinket")]
    public class Trinket : EquipableItem
    {
        protected override void Awake()
        {
            base.Awake();
            ItemCategory = ItemCategory.TRINKET;
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            ItemCategory = ItemCategory.TRINKET;
        }
    }
}
