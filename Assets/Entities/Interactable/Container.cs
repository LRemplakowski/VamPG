using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SunsetSystems.Inventory;

namespace Entities.Interactable
{
    [RequireComponent(typeof(ItemStorage))]
    public class Container : InteractableEntity
    {
        [SerializeField]
        private ItemStorage _myInventory;

        private void Awake()
        {
            if (!_myInventory)
                _myInventory = GetComponent<ItemStorage>();
        }

        public override void Interact()
        {

            base.Interact();
        }
    }
}
