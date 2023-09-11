using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Inventory.Data
{
    [CreateAssetMenu(fileName = "New Trousers", menuName = "Sunset Inventory/Items/Trousers")]
    public class Trousers : AbstractArmor
    {
        private void Awake()
        {
            ItemCategory = ItemCategory.TROUSERS;
        }
    }
}
