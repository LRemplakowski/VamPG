using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Inventory.Data
{
    [CreateAssetMenu(fileName = "New Gloves", menuName = "Sunset Inventory/Items/Gloves")]
    public class Gloves : AbstractArmor
    {
        private void Awake()
        {
            _itemCategory = ItemCategory.GLOVES;
        }
    }
}
