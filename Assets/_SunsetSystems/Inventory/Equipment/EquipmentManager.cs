using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using SunsetSystems.Core.Database;
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
                    if (slot.TryEquipItem(item, out var unequipped))
                    {
                        if (unequipped != null && unequipped.ReadableID != slot.DefaultItemID)
                            ItemUnequipped?.InvokeSafe(unequipped);
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
                EquipmentSlots = InitializeEquipmentSlots();
                return;
            }
            EquipmentSlots ??= InitializeEquipmentSlots();

            foreach (EquipmentSlotID key in template.EquipmentSlotsData.Keys)
            {
                if (template.EquipmentSlotsData.TryGetValue(key, out var templateSlot) && EquipmentSlots.TryGetValue(key, out var mySlot))
                {
                    if (ItemDatabase.Instance.TryGetEntryByReadableID(templateSlot, out var item) && item is IWearable wearable && mySlot.TryEquipItem(wearable, out var _))
                    {
                        Debug.Log($"Injected item {mySlot.GetEquippedItem()} into equipment manager!");
                        ItemEquipped?.InvokeSafe(EquipmentSlots[key].GetEquippedItem());
                    }
                }
            }
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode is false)
            {
                UnityEditor.EditorUtility.SetDirty(this);
            }
#endif
        }

        private Dictionary<EquipmentSlotID, IEquipmentSlot> InitializeEquipmentSlots()
        {
            return new()
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
}
