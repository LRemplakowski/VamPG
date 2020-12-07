using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
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
        return true;
    }

    public virtual void Remove(InventoryItem item)
    {
        items.Remove(item);
    }
}
