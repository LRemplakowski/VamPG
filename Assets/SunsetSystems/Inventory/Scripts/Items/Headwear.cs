using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Inventory.Data
{
    [CreateAssetMenu(fileName = "New Headwear", menuName = "Sunset Inventory/Items/Headwear")]
    public class Headwear : AbstractArmor
    {
        private void Awake()
        {
            ItemCategory = ItemCategory.HEADWEAR;
        }
    }
}
