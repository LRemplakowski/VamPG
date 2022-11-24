using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SunsetSystems.Inventory;
using SunsetSystems.Inventory.UI;

namespace SunsetSystems.Entities.Interactable
{
    [RequireComponent(typeof(ItemStorage))]
    public class Container : InteractableEntity
    {
        [SerializeField]
        private ItemStorage _myInventory;
        private ContainerGUI _containerGUI;

        protected override void Awake()
        {
            if (!_myInventory)
                _myInventory = GetComponent<ItemStorage>();
        }

        protected override void HandleInteraction()
        {
            OpenContainer();
        }

        private void OpenContainer()
        {
            if (this.TryFindFirstGameObjectWithTag(TagConstants.CONTAINER_GUI, out GameObject guiGO))
            {
                if (guiGO.TryGetComponent(out _containerGUI))
                {
                    _containerGUI.OpenContainerGUI(_myInventory, "TEST");
                }
            }
        }
    }
}
