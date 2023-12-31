using Sirenix.OdinInspector;
using Sirenix.Serialization;
using SunsetSystems.Entities.Characters.Interfaces;
using SunsetSystems.Equipment;
using SunsetSystems.Inventory;
using SunsetSystems.Inventory.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UltEvents;
using UnityEngine;

namespace SunsetSystems.Entities.Characters
{
    public class EquipmentManager : SerializedMonoBehaviour, IEquipmentManager
    {
        [field: Title("Data")]
        [field: SerializeField]
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
                EquipmentSlots = new();
                EquipmentSlots.Add(EquipmentSlotID.PrimaryWeapon, new EquipmentSlot(ItemCategory.WEAPON, EquipmentSlotID.PrimaryWeapon));
                EquipmentSlots.Add(EquipmentSlotID.SecondaryWeapon, new EquipmentSlot(ItemCategory.WEAPON, EquipmentSlotID.SecondaryWeapon));
                EquipmentSlots.Add(EquipmentSlotID.Chest, new EquipmentSlot(ItemCategory.CLOTHING, EquipmentSlotID.Chest));
                EquipmentSlots.Add(EquipmentSlotID.Boots, new EquipmentSlot(ItemCategory.SHOES, EquipmentSlotID.Boots));
                EquipmentSlots.Add(EquipmentSlotID.Hands, new EquipmentSlot(ItemCategory.GLOVES, EquipmentSlotID.Hands));
                EquipmentSlots.Add(EquipmentSlotID.Trinket, new EquipmentSlot(ItemCategory.TRINKET, EquipmentSlotID.Trinket));
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
                        IEquipableItem previousItem = slot.GetEquippedItem();
                        if (slot.TryUnequipItem(previousItem))
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
                return slot.TryUnequipItem(slot.GetEquippedItem());
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
            if (EquipmentSlots == null)
                EquipmentSlots = new();
            foreach (EquipmentSlotID key in template.EquipmentSlotsData.Keys)
            {
                EquipmentSlots[key] = new EquipmentSlot(template.EquipmentSlotsData[key]);
            }
        }
    }
}
