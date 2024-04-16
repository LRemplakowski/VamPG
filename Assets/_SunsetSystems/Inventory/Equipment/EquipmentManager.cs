using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using SunsetSystems.Entities.Characters.Interfaces;
using SunsetSystems.Equipment;
using SunsetSystems.Inventory.Data;
using UltEvents;
using UnityEngine;

namespace SunsetSystems.Entities.Characters
{
    public class EquipmentManager : SerializedMonoBehaviour, IEquipmentManager
    {
        [field: Title("Data")]
        [field: SerializeField, DictionaryDrawerSettings(IsReadOnly = true)]
        public Dictionary<EquipmentSlotID, IEquipmentSlot> EquipmentSlots { get; private set; }

        [Title("Events")]
        public UltEvent<IEquipableItem> ItemEquipped;
        public UltEvent<IEquipableItem> ItemUnequipped;

        private void Start()
        {
            foreach (IEquipableItem item in EquipmentSlots.Values.Select(slot => slot.GetEquippedItem()))
            {
                if (item == null)
                    continue;
                ItemEquipped?.InvokeSafe(item);
            }
        }

        private void OnValidate()
        {
            if (EquipmentSlots == null || EquipmentSlots.Count < Enum.GetValues(typeof(EquipmentSlotID)).Length-1)
            {
                EquipmentSlots = new()
                {
                    { EquipmentSlotID.PrimaryWeapon, new EquipmentSlot(EquipmentSlotID.PrimaryWeapon) },
                    { EquipmentSlotID.SecondaryWeapon, new EquipmentSlot(EquipmentSlotID.SecondaryWeapon) },
                    { EquipmentSlotID.Chest, new EquipmentSlot(EquipmentSlotID.Chest) },
                    { EquipmentSlotID.Boots, new EquipmentSlot(EquipmentSlotID.Boots) },
                    { EquipmentSlotID.Hands, new EquipmentSlot(EquipmentSlotID.Hands) },
                    { EquipmentSlotID.Trinket, new EquipmentSlot(EquipmentSlotID.Trinket) }
                };
            }
        }

        public bool EquipItem(EquipmentSlotID slotID, IEquipableItem item)
        {
            if (ValidateItem(slotID, item))
            {
                if (EquipmentSlots.TryGetValue(slotID, out IEquipmentSlot slot))
                {
                    if (slot.GetEquippedItem() != null)
                    {
                        if (slot.TryUnequipItem(out var previousItem))
                        {
                            ItemUnequipped?.InvokeSafe(previousItem);
                        }
                    }
                    if (slot.TryEquipItem(item))
                    {
                        ItemEquipped?.InvokeSafe(item);
                        return true;
                    }
                }
            }
            return false;
        }

        public bool UnequipItem(EquipmentSlotID slotID)
        {
            if (EquipmentSlots.TryGetValue(slotID, out IEquipmentSlot slot))
            {
                return slot.TryUnequipItem(out var _);
            }
            return false;
        }

        private bool ValidateItem(EquipmentSlotID slotID, IEquipableItem item)
        {
            if (EquipmentSlots.TryGetValue(slotID, out IEquipmentSlot slot))
            {
                return slot.AcceptedCategory == item.ItemCategory;
            }
            return false;
        }

        public void CopyFromTemplate(ICreatureTemplate template)
        {
            if (template == null || template.EquipmentSlotsData == null)
            {
                EquipmentSlots = new()
                {
                    { EquipmentSlotID.PrimaryWeapon, new EquipmentSlot(EquipmentSlotID.PrimaryWeapon) },
                    { EquipmentSlotID.SecondaryWeapon, new EquipmentSlot(EquipmentSlotID.SecondaryWeapon) },
                    { EquipmentSlotID.Chest, new EquipmentSlot(EquipmentSlotID.Chest) },
                    { EquipmentSlotID.Boots, new EquipmentSlot(EquipmentSlotID.Boots) },
                    { EquipmentSlotID.Hands, new EquipmentSlot(EquipmentSlotID.Hands) },
                    { EquipmentSlotID.Trinket, new EquipmentSlot(EquipmentSlotID.Trinket) }
                };
                return;
            }
            if (EquipmentSlots == null)
                EquipmentSlots = new();
            foreach (EquipmentSlotID key in template.EquipmentSlotsData.Keys)
            {
                EquipmentSlots[key] = new EquipmentSlot(template.EquipmentSlotsData[key]);
            }
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode is false)
            {
                UnityEditor.EditorUtility.SetDirty(this);
            }
#endif
        }
    }
}
