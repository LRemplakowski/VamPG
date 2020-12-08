using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment")]
public class EquipmentPiece : InventoryItem
{
    public EquipmentSlot slot;
    public SkinnedMeshRenderer meshRenderer;

    public override void Use()
    {
        base.Use();
        Debug.Log("Item: " + this + "\nEquipment manager instance: " + EquipmentManager.instance);
        EquipmentManager.instance.Equip(this);
        RemoveFromInventory();
    }
}
