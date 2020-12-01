using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryButton : MonoBehaviour
{
    public Image itemIcon;

    private InventoryItem item;

    private void Awake()
    {
        if(itemIcon == null)
        {
            List<Image> images = new List<Image>(GetComponentsInChildren<Image>(true));
            foreach(Image image in images)
            {
                if(image.CompareTag("Inventory icon"))
                {
                    itemIcon = image;
                    break;
                }
            }
        }
    }

    public void OnClick()
    {
        Debug.Log("Inventory slot clicked!");
        if(item != null)
        {

        }
    }

    public void AddItem(InventoryItem item)
    {
        this.item = item;

        itemIcon.sprite = item.icon;
        itemIcon.enabled = true;
    }

    public void ClearSlot()
    {
        this.item = null;

        itemIcon.sprite = null;
        itemIcon.enabled = false;
    }
}
