using Sirenix.OdinInspector;
using SunsetSystems.Entities.Characters.Interfaces;
using SunsetSystems.Equipment;
using SunsetSystems.Inventory;
using SunsetSystems.Inventory.Data;
using System;
using UltEvents;
using UnityEngine;

namespace SunsetSystems.Entities.Characters
{
    public class EquipmentManager : SerializedMonoBehaviour, IEquipment
    {
        [Title("Data")]
        [SerializeField]
        public EquipmentData EquipmentData { get; private set; }

        [Title("Events")]
        public UltEvent<IEquipableItem> ItemEquipped;
        public UltEvent<IEquipableItem> ItemUnequipped;

        [Title("Editor Utility")]
        [Button]
        public void EquipItem(EquipmentSlotID slotID, IEquipableItem item)
        {
            if (ValidateItem(slotID, item))
            {
                if (EquipmentData.EquipmentSlots.TryGetValue(slotID, out IEquipmentSlot slot))
                {
                    if (slot.GetEquippedItem() != null)
                    {
                        IEquipableItem previousItem = slot.GetEquippedItem();
                        if (slot.TryUnequipItem(previousItem))
                        {
                            ItemUnequipped?.InvokeSafe(previousItem);
                        }
                    }
                    if (slot.TryEquipItem(item))
                    {
                        ItemEquipped?.InvokeSafe(item);
                    }
                }
            }
        }

        [Button]
        public void UnequipItem(EquipmentSlotID slotID)
        {
            if (EquipmentData.EquipmentSlots.TryGetValue(slotID, out IEquipmentSlot slot))
            {
                slot.TryUnequipItem(slot.GetEquippedItem());
            }
        }

        private bool ValidateItem(EquipmentSlotID slotID, IEquipableItem item)
        {
            if (EquipmentData.EquipmentSlots.TryGetValue(slotID, out IEquipmentSlot slot))
            {
                return slot.AcceptedCategory == item.ItemCategory;
            }
            return false;
        }

        public void CopyFromTemplate(ICreatureTemplate template)
        {
            EquipmentData = template.EquipmentData;
        }
    }
}
