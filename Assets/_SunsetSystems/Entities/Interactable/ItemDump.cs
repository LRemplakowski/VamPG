using Sirenix.OdinInspector;
using SunsetSystems.ActionSystem;
using SunsetSystems.Inventory;
using SunsetSystems.Inventory.Data;
using UnityEngine;

namespace SunsetSystems.Entities.Interactable
{
    public class ItemDump : SerializedMonoBehaviour, IInteractionHandler
    {
        [SerializeField]
        private IBaseItem itemTemplate;
        [SerializeField, Min(1)]
        private int requiredAmount;

        public bool HandleInteraction(IActionPerformer interactee)
        {
            return InventoryManager.Instance.TakeItemFromPlayer(itemTemplate, requiredAmount);
        }
    }
}
