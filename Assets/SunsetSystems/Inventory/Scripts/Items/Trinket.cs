using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Inventory.Data
{
    [CreateAssetMenu(fileName = "New Trinket", menuName = "Sunset Inventory/Items/Trinket")]
    public class Trinket : EquipableItem
    {
        private void Awake()
        {
            ItemCategory = ItemCategory.TRINKET;
        }
    }
}
