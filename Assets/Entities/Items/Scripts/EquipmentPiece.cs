using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment")]
public class EquipmentPiece : InventoryItem
{
    public EquipmentSlot slot;
    public SkinnedMeshRenderer meshRenderer;
    public EquipmentMeshRegion[] coveredMeshRegions;

    public override void Use()
    {
        base.Use();
        Debug.Log("Item: " + this + "\nEquipment manager instance: " + EquipmentManager.Instance);
        EquipmentManager.Instance.Equip(this);
        RemoveFromInventory();
    }
}
