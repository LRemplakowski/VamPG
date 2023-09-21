using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SunsetSystems.Inventory;
using SunsetSystems.Inventory.Data;
using System;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

namespace SunsetSystems.Entities.Characters
{
    [Serializable]
    public struct EquipmentData
    {
        [OdinSerialize]
        public Dictionary<EquipmentSlotID, IEquipmentSlot> EquipmentSlots;
        [ShowInInspector, ReadOnly]
        private EquipmentSlotID _selectedWeapon;

        public static Dictionary<EquipmentSlotID, IEquipmentSlot> GetSlotsPreset()
        {
            Dictionary<EquipmentSlotID, IEquipmentSlot> equipmentSlots = new();
            equipmentSlots.Add(EquipmentSlotID.PrimaryWeapon, new EquipmentSlot(ItemCategory.WEAPON, EquipmentSlotID.PrimaryWeapon));
            equipmentSlots.Add(EquipmentSlotID.SecondaryWeapon, new EquipmentSlot(ItemCategory.WEAPON, EquipmentSlotID.SecondaryWeapon));
            equipmentSlots.Add(EquipmentSlotID.Chest, new EquipmentSlot(ItemCategory.CLOTHING, EquipmentSlotID.Chest));
            equipmentSlots.Add(EquipmentSlotID.Boots, new EquipmentSlot(ItemCategory.SHOES, EquipmentSlotID.Boots));
            equipmentSlots.Add(EquipmentSlotID.Hands, new EquipmentSlot(ItemCategory.GLOVES, EquipmentSlotID.Hands));
            equipmentSlots.Add(EquipmentSlotID.Trinket, new EquipmentSlot(ItemCategory.TRINKET, EquipmentSlotID.Trinket));
            return equipmentSlots;
        }

        public EquipmentData(InventoryConfig config)
        {
            EquipmentSlots = GetSlotsPreset();
            _selectedWeapon = EquipmentSlotID.PrimaryWeapon;
            foreach (EquipmentSlotID key in config.Equipment.EquipmentSlots.Keys)
            {
                if (EquipmentSlots.ContainsKey(key))
                {
                    EquipmentSlots[key] = config.Equipment.EquipmentSlots[key];
                }    
            }
        }

        public EquipmentData(EquipmentData existing)
        {
            EquipmentSlots = new();
            _selectedWeapon = existing._selectedWeapon;
            foreach (EquipmentSlotID id in Enum.GetValues(typeof(EquipmentSlotID)))
            {
                EquipmentSlots.Add(id, new EquipmentSlot(existing.EquipmentSlots[id]));
            }
        }

        public void SetSelectedWeapon(SelectedWeapon weapon)
        {
            _selectedWeapon = weapon switch
            {
                SelectedWeapon.Primary => EquipmentSlotID.PrimaryWeapon,
                SelectedWeapon.Secondary => EquipmentSlotID.SecondaryWeapon,
                _ => EquipmentSlotID.PrimaryWeapon,
            };
        }

        public Weapon GetSelectedWeapon()
        {
            return EquipmentSlots[_selectedWeapon].GetEquippedItem() as Weapon;
        }

        public Weapon GetPrimaryWeapon()
        {
            return EquipmentSlots[EquipmentSlotID.PrimaryWeapon].GetEquippedItem() as Weapon;
        }

        public Weapon GetSecondaryWeapon()
        {
            return EquipmentSlots[EquipmentSlotID.SecondaryWeapon].GetEquippedItem() as Weapon;
        }
    }

    public enum SelectedWeapon
    {
        Primary, Secondary
    }

    public enum EquipmentSlotID
    {
        PrimaryWeapon, SecondaryWeapon, Chest, Boots, Hands, Trinket
    }
}
