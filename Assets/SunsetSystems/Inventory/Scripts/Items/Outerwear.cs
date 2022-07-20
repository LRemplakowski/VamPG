using UnityEngine;

namespace SunsetSystems.Inventory.Data
{
    [CreateAssetMenu(fileName = "New Outerwear", menuName = "Sunset Inventory/Items/Outerwear")]
    public class Outerwear : AbstractArmor
    {
        private void Awake()
        {
            _itemCategory = ItemCategory.OUTER_CLOTHING;
        }
    }
}
