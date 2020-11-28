using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory/Creature Inventory")]
public class Inventory : ScriptableObject
{
    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    public int size = 20;
    public List<InventoryItem> items = new List<InventoryItem>();

    public bool Add(InventoryItem item)
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
            if(onItemChangedCallback != null)
                onItemChangedCallback.Invoke();
        }
        return true;
    }

    public void Remove(InventoryItem item)
    {
        items.Remove(item);

        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke();
    }
}
