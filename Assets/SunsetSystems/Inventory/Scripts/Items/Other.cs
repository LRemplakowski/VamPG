using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Inventory.Data
{
    [CreateAssetMenu(fileName = "New Other Item", menuName = "Sunset Inventory/Items/Other")]
    public class Other : BaseItem
    {
        private void Awake()
        {
            _itemCategory = ItemCategory.OTHER;
        }
    }
}
