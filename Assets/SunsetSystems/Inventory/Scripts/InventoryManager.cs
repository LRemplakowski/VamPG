using SunsetSystems.Inventory.Data;
using SunsetSystems.Management;
using UnityEngine;

namespace SunsetSystems.Inventory
{
    [RequireComponent(typeof(Inventory))]
    public class InventoryManager : Manager
    {
        [field: SerializeField]
        public Inventory PlayerInventory { get; private set; }

        private void Awake()
        {
            if (!PlayerInventory)
                PlayerInventory = GetComponent<Inventory>();
        }
    }
}
