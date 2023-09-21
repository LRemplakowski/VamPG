using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using SunsetSystems.Inventory.Data;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Entities.Characters.Editor
{
    public class EquipmentDataValueDrawer : OdinValueDrawer<EquipmentData>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            EquipmentData data = this.ValueEntry.SmartValue;
            ICollection<EquipmentSlotID> slotKeys = data.EquipmentSlots?.Keys;
            if (slotKeys == null)
            {
                data.EquipmentSlots = EquipmentData.GetSlotsPreset();
                slotKeys = data.EquipmentSlots.Keys;
            }

            foreach (EquipmentSlotID key in slotKeys)
            {
                SirenixEditorGUI.BeginInlineBox();
                string itemName = "Empty";
                if (data.EquipmentSlots[key] != null)
                {
                    IEquipableItem item = data.EquipmentSlots[key].GetEquippedItem();
                    if (item != null)
                        itemName = item.Name;
                }
                SirenixEditorFields.TextField(new GUIContent($"{key}"), itemName);
                SirenixEditorGUI.EndInlineBox();
            }

            this.ValueEntry.SmartValue = data;
        }
    }
}
