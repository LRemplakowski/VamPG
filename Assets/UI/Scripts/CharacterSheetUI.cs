using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSheetUI : UIWindow
{
    private EquipmentDisplay equipmentDisplay;

    private void Awake()
    {
        if (equipmentDisplay == null)
        {
            equipmentDisplay = GetComponentInChildren<EquipmentDisplay>();
        }
    }

    private void OnEnable()
    {
        equipmentDisplay.UpdateDisplay(null, null);
    }
}
