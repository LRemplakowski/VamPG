using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory/Inventory")]
public class Inventory : ScriptableObject
{
    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    public int size = 20;
    public List<InventoryItem> items = new List<InventoryItem>();

    public virtual bool Add(InventoryItem item)
    {
        if(!item.isDefaultItem)
        {
            Debug.Log("Inventory size: " + size + "\nCurrent item count: " + items.Count);
            if(items.Count >= size)
            {
                Debug.Log("Not enough room");
                return false;
            }

            items.Add(item);
        }
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
        return true;
    }

    public virtual void Remove(InventoryItem item)
    {
        items.Remove(item);
        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke();
    }
}
