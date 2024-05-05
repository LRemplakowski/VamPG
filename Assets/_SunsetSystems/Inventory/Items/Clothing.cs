using UnityEngine;

namespace SunsetSystems.Inventory.Data
{
    [CreateAssetMenu(fileName = "New Clothing", menuName = "Sunset Inventory/Items/Clothing")]
    public class Clothing : WearableItem
    {
        private void Awake()
        {
            ItemCategory = ItemCategory.CLOTHING;
        }

        private void OnValidate()
        {
            ItemCategory = ItemCategory.CLOTHING;
        }
    }
}
