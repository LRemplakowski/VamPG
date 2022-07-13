using SunsetSystems.Inventory.Data;
using SunsetSystems.Utils;
using UnityEngine;

namespace SunsetSystems.Inventory
{
    [RequireComponent(typeof(ItemStorage))]
    public class InventoryManager : Singleton<InventoryManager>
    {
        [field: SerializeField]
        public ItemStorage PlayerInventory { get; private set; }

        protected override void Awake()
        {
            if (!PlayerInventory)
                PlayerInventory = GetComponent<ItemStorage>();
            if (!PlayerInventory)
                PlayerInventory = gameObject.AddComponent(typeof(ItemStorage)) as ItemStorage;
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
