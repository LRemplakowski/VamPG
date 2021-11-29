using Entities.Characters;
using System.Collections;
using System.Collections.Generic;
using Systems.Management;
using UnityEngine;

public class PlayerInventoryUI : UIWindow
{
    public Transform itemsParent;
    [HideInInspector]
    public InventoryItem selected;

    private Inventory playerInventory;
    private InventorySlot[] slots;

    private void Start()
    {
        if(playerInventory == null)
        {
            playerInventory = FindObjectOfType<Player>().GetInventory();
        }
        if (playerInventory != null)
            playerInventory.onItemChangedCallback += UpdateUI;
        slots = itemsParent.GetComponentsInChildren<InventorySlot>();
        UpdateUI();
    }

    private void UpdateUI()
    {
        Debug.Log("Updating inventory!");
        for(int i=0; i < slots.Length; i++)
        {
            if(i < playerInventory.items.Count)
            {
                slots[i].AddItem(playerInventory.items[i]);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }
}
