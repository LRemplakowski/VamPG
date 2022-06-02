using SunsetSystems.Inventory.Data;
using SunsetSystems.Management;
using UnityEngine;

namespace SunsetSystems.Inventory
{
    [RequireComponent(typeof(ItemStorage))]
    public class InventoryManager : Manager
    {
        [field: SerializeField]
        public ItemStorage PlayerInventory { get; private set; }

        private void Awake()
        {
            if (!PlayerInventory)
                PlayerInventory = GetComponent<ItemStorage>();
        }

        public void TransferItem(ItemStorage from, ItemStorage to, InventoryEntry item)
        {
            if (from.TryRemoveItem(item))
            {
                to.AddItem(item);
            }
        }
    }
}
