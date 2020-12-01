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
            if (onItemChangedCallback != null)
                onItemChangedCallback.Invoke();
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
