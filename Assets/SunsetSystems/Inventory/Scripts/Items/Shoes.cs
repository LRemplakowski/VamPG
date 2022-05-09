using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Inventory.Data
{
    [CreateAssetMenu(fileName = "New Shoes", menuName = "Sunset Inventory/Items/Shoes")]
    public class Shoes : AbstractArmor
    {
        private void Awake()
        {
            _itemCategory = ItemCategory.SHOES;
        }
    }
}
