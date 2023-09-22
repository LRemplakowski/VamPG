using SunsetSystems.Entities.Characters;
using SunsetSystems.Entities.Characters.Interfaces;
using SunsetSystems.Inventory.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Equipment
{
    public interface IEquipment
    {
        EquipmentData EquipmentData { get; }

        void EquipItem(EquipmentSlotID slotID, IEquipableItem item);
        void UnequipItem(EquipmentSlotID slotID);
        void CopyFromTemplate(ICreatureTemplate template);
    }
}
