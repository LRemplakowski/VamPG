using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDropdown : CustomContextMenu
{
    public override void InvokeMenu(Vector3 position, UIWindow parent)
    {
        PlayerInventoryUI window = (PlayerInventoryUI)parent;
        base.InvokeMenu(position, window);
        ContextAction.item = window.selected;
        Debug.Log("Context menu item set to " + window.selected);
    }
}
