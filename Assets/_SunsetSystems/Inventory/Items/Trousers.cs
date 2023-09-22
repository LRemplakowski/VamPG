using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Inventory.Data
{
    [CreateAssetMenu(fileName = "New Trousers", menuName = "Sunset Inventory/Items/Trousers")]
    public class Trousers : WearableItem
    {
        private void Awake()
        {
            ItemCategory = ItemCategory.TROUSERS;
        }
    }
}
