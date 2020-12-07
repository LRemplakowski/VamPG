using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : Inventory
{
    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    public override bool Add(InventoryItem item)
    {
        bool success = base.Add(item);
        if(success)
        {
            Debug.Log("PlayerInventory: Check for callback");
            if (onItemChangedCallback != null)
            {
                Debug.Log("Invoking inventory callback");
                onItemChangedCallback.Invoke();
            }
            else
            {
                Debug.Log("Inventory callback not found");
            }
                
        }
        return success;
    }

    public override void Remove(InventoryItem item)
    {
        base.Remove(item);
        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke();
    }
}
