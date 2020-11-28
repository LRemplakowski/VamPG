using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableItem : InteractableEntity
{
    public InventoryItem item;

    public override void Interact()
    {
        PickUp();
        base.Interact();
    }

    private void PickUp()
    {
        Debug.Log("Picking up " + gameObject);
        if(TargetedBy.inventory.Add(item))
        {
            Destroy(gameObject);
        }
    }
}
