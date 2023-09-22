using System.Collections.Generic;
using SunsetSystems.Inventory.Data;
using System;
using Sirenix.Serialization;
using SunsetSystems.Entities.Characters;

namespace SunsetSystems.Equipment
{
    [Serializable]
    public class EquipmentData : Object
    {
        [field: OdinSerialize]
        public Dictionary<EquipmentSlotID, IEquipmentSlot> EquipmentSlots { get; private set; }

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
            foreach (EquipmentSlotID id in existing.EquipmentSlots.Keys)
            {
                EquipmentSlots[id] = new EquipmentSlot(existing.EquipmentSlots[id]);
            }
        }

        public EquipmentData()
        {
            EquipmentSlots = GetSlotsPreset();
        }
    }
}
