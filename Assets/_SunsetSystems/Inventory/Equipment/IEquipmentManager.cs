using System.Collections.Generic;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Inventory.Data;

namespace SunsetSystems.Equipment
{
    public interface IEquipmentManager
    {
        Dictionary<EquipmentSlotID, IEquipmentSlot> EquipmentSlots { get; }

        EquipmentSlotID GetSlotForItem(IEquipableItem item);
        bool EquipItem(EquipmentSlotID slotID, IEquipableItem item, out IEquipableItem previouslyEquipped);
        bool UnequipItem(EquipmentSlotID slotID, out IEquipableItem unequipped);
        bool IsItemEquipped(IEquipableItem item);
        void CopyFromTemplate(ICreatureTemplate template);
    }
}
