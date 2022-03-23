using System.Collections;
using System.Collections.Generic;
using SunsetSystems.Management;
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
        References.Get<EquipmentManager>().Equip(this);
        RemoveFromInventory();
    }
}
