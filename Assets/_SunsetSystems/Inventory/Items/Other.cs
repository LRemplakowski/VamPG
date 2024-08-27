using UnityEngine;

namespace SunsetSystems.Inventory.Data
{
    [CreateAssetMenu(fileName = "New Other Item", menuName = "Sunset Inventory/Items/Other")]
    public class Other : BaseItem
    {
        protected override void Awake()
        {
            base.Awake();
            ItemCategory = ItemCategory.OTHER;
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            ItemCategory = ItemCategory.OTHER;
        }
    }
}
