using UnityEngine;

namespace SunsetSystems.Inventory.Data
{
    [CreateAssetMenu(fileName = "New Facewear", menuName = "Sunset Inventory/Items/Facewear")]
    public class Facewear : WearableItem
    {
        protected override void Awake()
        {
            base.Awake();
            ItemCategory = ItemCategory.FACEWEAR;
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            ItemCategory = ItemCategory.FACEWEAR;
        }
    }
}
