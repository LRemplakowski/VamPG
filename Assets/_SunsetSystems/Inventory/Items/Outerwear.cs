using UnityEngine;

namespace SunsetSystems.Inventory.Data
{
    [CreateAssetMenu(fileName = "New Outerwear", menuName = "Sunset Inventory/Items/Outerwear")]
    public class Outerwear : WearableItem
    {
        protected override void Awake()
        {
            base.Awake();
            ItemCategory = ItemCategory.OUTER_CLOTHING;
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            ItemCategory = ItemCategory.OUTER_CLOTHING;
        }
    }
}
