using UnityEngine;

namespace SunsetSystems.Inventory.Data
{
    [CreateAssetMenu(fileName = "New Clothing", menuName = "Sunset Inventory/Items/Clothing")]
    public class Clothing : AbstractArmor
    {
        private void Awake()
        {
            ItemCategory = ItemCategory.CLOTHING;
        }
    }
}
