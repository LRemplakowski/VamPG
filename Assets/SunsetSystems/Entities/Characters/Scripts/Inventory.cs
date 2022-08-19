using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory/Inventory")]
public class Inventory : ScriptableObject
{
    public delegate void OnItemChanged();
    public event OnItemChanged OnItemChangedCallback;

    public int size = 20;
    public List<InventoryItem> items = new();

    public virtual bool Add(InventoryItem item)
    {
        if (!item.isDefaultItem)
        {
            Debug.Log("Inventory size: " + size + "\nCurrent item count: " + items.Count);
            if (items.Count >= size)
            {
                Debug.Log("Not enough room");
                return false;
            }

            items.Add(item);
        }
        OnItemChangedCallback?.Invoke();
        return true;
    }

    public virtual void Remove(InventoryItem item)
    {
        items.Remove(item);
        if (OnItemChangedCallback != null)
            OnItemChangedCallback.Invoke();
    }
}
