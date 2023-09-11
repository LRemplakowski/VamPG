using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using SunsetSystems.Inventory.Data;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SunsetSystems.Entities.Characters.Editor
{
    public class EquipmentDataValueDrawer : OdinValueDrawer<EquipmentData>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            EquipmentData data = this.ValueEntry.SmartValue;
            ICollection<string> slotKeys = data.EquipmentSlots?.Keys;
            if (slotKeys == null)
            {
                data.EquipmentSlots = EquipmentData.GetSlotsPreset();
                slotKeys = data.EquipmentSlots.Keys;
            }

            foreach (string key in slotKeys)
            {
                SirenixEditorGUI.BeginInlineBox();
                string itemName = "Empty";
                if (data.EquipmentSlots[key] != null)
                {
                    EquipableItem item = data.EquipmentSlots[key].GetEquippedItem();
                    if (item != null)
                        itemName = item.name;
                }
                SirenixEditorFields.TextField(new GUIContent($"{key}"), itemName);
                SirenixEditorGUI.EndInlineBox();
            }

            this.ValueEntry.SmartValue = data;
        }
    }
}
