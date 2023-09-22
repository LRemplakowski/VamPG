using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Inventory.Data
{
    [CreateAssetMenu(fileName = "New Shoes", menuName = "Sunset Inventory/Items/Shoes")]
    public class Shoes : WearableItem
    {
        private void Awake()
        {
            ItemCategory = ItemCategory.SHOES;
        }
    }
}
