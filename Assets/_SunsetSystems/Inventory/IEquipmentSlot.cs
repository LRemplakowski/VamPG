using System;
using SunsetSystems.Inventory.Data;
using SunsetSystems.UI.Utils;

namespace SunsetSystems.Equipment
{
    public interface IEquipmentSlot : IUserInfertaceDataProvider<IEquipmentSlot>
    {
        string DefaultItemID { get; }

        ItemCategory AcceptedCategory { get; }

        IEquipableItem GetEquippedItem();

        EquipmentSlotID ID { get; }

        bool TryEquipItem(IEquipableItem itemToEquip, out IEquipableItem unequipped);

        bool TryUnequipItem(out IEquipableItem previouslyEquippedItem);
    }

    public enum EquipmentSlotID
    {
        PrimaryWeapon, SecondaryWeapon, Chest, Boots, Hands, Trinket, Invalid
    }
}
