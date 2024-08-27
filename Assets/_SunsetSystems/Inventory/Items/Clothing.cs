using UnityEngine;

namespace SunsetSystems.Inventory.Data
{
    [CreateAssetMenu(fileName = "New Clothing", menuName = "Sunset Inventory/Items/Clothing")]
    public class Clothing : WearableItem
    {
        protected override void Awake()
        {
            base.Awake();
            ItemCategory = ItemCategory.CLOTHING;
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            ItemCategory = ItemCategory.CLOTHING;
        }
    }
}
