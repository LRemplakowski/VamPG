using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SunsetSystems.Inventory;
using SunsetSystems.Inventory.Data;
using System;

namespace SunsetSystems.Entities.Characters
{
    [Serializable]
    public class EquipmentData
    {
        public const string SLOT_WEAPON_PRIMARY = "SLOT_WEAPON_PRIMARY";
        public const string SLOT_WEAPON_SECONDARY = "SLOT_WEAPON_SECONDARY";
        public const string SLOT_CHEST = "SLOT_CHEST";
        public const string SLOT_BOOTS = "SLOT_BOOTS";
        public const string SLOT_HANDS = "SLOT_HANDS";
        public const string SLOT_TRINKET = "SLOT_TRINKET";

        public Dictionary<string, EquipmentSlot> EquipmentSlots;

        private string _selectedWeapon;

        private static Dictionary<string, EquipmentSlot> GetSlotsPreset()
        {
            Dictionary<string, EquipmentSlot> equipmentSlots = new();
            equipmentSlots.Add(SLOT_WEAPON_PRIMARY, new EquipmentSlot(ItemCategory.WEAPON, "Primary Weapon", SLOT_WEAPON_PRIMARY));
            equipmentSlots.Add(SLOT_WEAPON_SECONDARY, new EquipmentSlot(ItemCategory.WEAPON, "Secondary Weapon", SLOT_WEAPON_SECONDARY));
            equipmentSlots.Add(SLOT_CHEST, new EquipmentSlot(ItemCategory.CLOTHING, "Chest", SLOT_CHEST));
            equipmentSlots.Add(SLOT_BOOTS, new EquipmentSlot(ItemCategory.SHOES, "Boots", SLOT_BOOTS));
            equipmentSlots.Add(SLOT_HANDS, new EquipmentSlot(ItemCategory.GLOVES, "Hands", SLOT_HANDS));
            equipmentSlots.Add(SLOT_TRINKET, new EquipmentSlot(ItemCategory.TRINKET, "Trinket", SLOT_TRINKET));
            return equipmentSlots;
        }

        public EquipmentData()
        {
            EquipmentSlots = GetSlotsPreset();
        }

        public EquipmentData(InventoryConfig config)
        {
            EquipmentSlots = GetSlotsPreset();
            _selectedWeapon = SLOT_WEAPON_PRIMARY;
            foreach (string key in config.Equipment.EquipmentSlots.Keys)
            {
                if (EquipmentSlots.ContainsKey(key))
                {
                    EquipmentSlots[key] = config.Equipment.EquipmentSlots[key];
                }    
            }
        }

        public void SetSelectedWeapon(SelectedWeapon weapon)
        {
            _selectedWeapon = weapon switch
            {
                SelectedWeapon.Primary => SLOT_WEAPON_PRIMARY,
                SelectedWeapon.Secondary => SLOT_WEAPON_SECONDARY,
                _ => SLOT_WEAPON_PRIMARY,
            };
        }

        public Weapon GetSelectedWeapon()
        {
            return EquipmentSlots[_selectedWeapon].GetEquippedItem() as Weapon;
        }

        public Weapon GetPrimaryWeapon()
        {
            return EquipmentSlots[SLOT_WEAPON_PRIMARY].GetEquippedItem() as Weapon;
        }

        public Weapon GetSecondaryWeapon()
        {
            return EquipmentSlots[SLOT_WEAPON_SECONDARY].GetEquippedItem() as Weapon;
        }

        public static List<string> GetSlotIDsFromItemCategory(ItemCategory category)
        {
            List<string> result = new();
            switch (category)
            {
                case ItemCategory.OTHER:
                    break;
                case ItemCategory.WEAPON:
                    result.Add(SLOT_WEAPON_PRIMARY);
                    result.Add(SLOT_WEAPON_SECONDARY);
                    break;
                case ItemCategory.CLOTHING:
                    result.Add(SLOT_CHEST);
                    break;
                case ItemCategory.OUTER_CLOTHING:
                    break;
                case ItemCategory.HEADWEAR:
                    break;
                case ItemCategory.FACEWEAR:
                    break;
                case ItemCategory.GLOVES:
                    result.Add(SLOT_HANDS);
                    break;
                case ItemCategory.SHOES:
                    result.Add(SLOT_BOOTS);
                    break;
                case ItemCategory.TROUSERS:
                    break;
                case ItemCategory.CONSUMABLE:
                    break;
                case ItemCategory.TRINKET:
                    result.Add(SLOT_TRINKET);
                    break;
            }
            return result;
        }
    }

    public enum SelectedWeapon
    {
        Primary, Secondary
    }
}
