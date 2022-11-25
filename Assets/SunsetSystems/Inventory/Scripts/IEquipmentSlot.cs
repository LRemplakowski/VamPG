using SunsetSystems.Inventory.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Inventory
{
    public interface IEquipmentSlot
    {
        ItemCategory AcceptedCategory { get; }

        EquipableItem GetEquippedItem();

        bool TryEquipItem(EquipableItem item);

        bool TryUnequipItem(EquipableItem item);
    }
}
