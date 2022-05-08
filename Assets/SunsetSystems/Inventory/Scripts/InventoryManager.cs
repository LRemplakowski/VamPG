using SunsetSystems.Inventory.Data;
using UnityEngine;

namespace SunsetSystems.Inventory
{
    public class InventoryManager : MonoBehaviour
    {
        public void TransferItem(BaseItem item, Inventory from, Inventory target)
        {
            from.RemoveItem(item);
            target.AddItem(item);
        }
    }
}
