using System;
using System.Collections.Generic;
using Sirenix.Serialization;
using SunsetSystems.Entities.Characters;

namespace SunsetSystems.Equipment
{
    [Serializable]
    public class EquipmentData
    {
        [field: OdinSerialize]
        public Dictionary<EquipmentSlotID, IEquipmentSlot> EquipmentSlots { get; private set; }

        public static Dictionary<EquipmentSlotID, IEquipmentSlot> GetSlotsPreset()
        {
            Dictionary<EquipmentSlotID, IEquipmentSlot> equipmentSlots = new()
            {
                { EquipmentSlotID.PrimaryWeapon, new EquipmentSlot(EquipmentSlotID.PrimaryWeapon) },
                { EquipmentSlotID.SecondaryWeapon, new EquipmentSlot(EquipmentSlotID.SecondaryWeapon) },
                { EquipmentSlotID.Chest, new EquipmentSlot(EquipmentSlotID.Chest) },
                { EquipmentSlotID.Boots, new EquipmentSlot(EquipmentSlotID.Boots) },
                { EquipmentSlotID.Hands, new EquipmentSlot(EquipmentSlotID.Hands) },
                { EquipmentSlotID.Trinket, new EquipmentSlot(EquipmentSlotID.Trinket) }
            };
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
