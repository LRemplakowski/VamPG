using SunsetSystems.Entities.Characters;
using SunsetSystems.Entities.Characters.Interfaces;
using SunsetSystems.Inventory;
using SunsetSystems.Inventory.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Equipment
{
    public interface IEquipmentManager
    {
        Dictionary<EquipmentSlotID, IEquipmentSlot> EquipmentSlots { get; }

        bool EquipItem(EquipmentSlotID slotID, IEquipableItem item);
        bool UnequipItem(EquipmentSlotID slotID);
        void CopyFromTemplate(ICreatureTemplate template);
    }
}
