using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Inventory.Data
{
    [CreateAssetMenu(fileName = "New Gloves", menuName = "Sunset Inventory/Items/Gloves")]
    public class Gloves : WearableItem
    {
        private void Awake()
        {
            ItemCategory = ItemCategory.GLOVES;
        }
    }
}
