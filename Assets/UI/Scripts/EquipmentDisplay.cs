using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentDisplay : ExposableMonobehaviour
{
    private EquipmentDisplaySlot[] displaySlots;

    private EquipmentManager equipmentManager;

    private void Awake()
    {
        displaySlots = GetComponentsInChildren<EquipmentDisplaySlot>(true);
        equipmentManager = EquipmentManager.instance;
    }

    private void OnEnable()
    {
        equipmentManager.onEquipmentChanged += UpdateDisplay;
    }

    private void OnDisable()
    {
        equipmentManager.onEquipmentChanged -= UpdateDisplay;
    }

    public void UpdateDisplay(EquipmentPiece newItem, EquipmentPiece oldItem)
    {
        //Debug.Log("Update display: "+newItem);
        foreach(EquipmentDisplaySlot s in displaySlots)
        {
            EquipmentPiece item = equipmentManager.GetItemInSlot(s.equipmentSlot);
            s.DisplayItem(item);
        }
    }
}
