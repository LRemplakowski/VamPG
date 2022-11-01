using UnityEngine;

namespace SunsetSystems.Inventory.Data
{
    [CreateAssetMenu(fileName = "New Facewear", menuName = "Sunset Inventory/Items/Facewear")]
    public class Facewear : AbstractArmor
    {
        private void Awake()
        {
            ItemCategory = ItemCategory.FACEWEAR;
        }
    }
}
