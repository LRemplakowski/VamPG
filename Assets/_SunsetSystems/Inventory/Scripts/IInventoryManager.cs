using SunsetSystems.Entities.Characters;
using SunsetSystems.Inventory.Data;
using System.Collections.Generic;

namespace SunsetSystems.Inventory
{
    public interface IInventoryManager
    {
        void AddCoterieMemberEquipment(string creatureID, CreatureData creatureData);
        void AddEntryToPlayerInventory(InventoryEntry entry);
        List<InventoryEntry> GetPlayerInventoryContents();
        bool TryGetEquipmentData(string characterKey, out EquipmentData data);
        bool TryEquipItemInSlot(string characterID, string slotID, EquipableItem item);
        bool TryEquipItem(string characterKey, EquipableItem item);
        bool TryUnequipItemFromSlot(string characterID, string slotID);
        void TransferItem(ItemStorage from, ItemStorage to, InventoryEntry item);
        void AddMoney(int value);
        bool TryRemoveMoney(int value);
        int GetMoney();
        void SetMoney(int value);
    }
}
