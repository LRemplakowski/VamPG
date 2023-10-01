using SunsetSystems.Entities.Characters;
using SunsetSystems.Inventory.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Equipment
{
    public interface IEquipmentSlot
    {
        ItemCategory AcceptedCategory { get; }

        IEquipableItem GetEquippedItem();

        EquipmentSlotID ID { get; }

        bool TryEquipItem(IEquipableItem item);

        bool TryUnequipItem(IEquipableItem item);
    }

    public enum EquipmentSlotID
    {
        PrimaryWeapon, SecondaryWeapon, Chest, Boots, Hands, Trinket, Invalid
    }
}
