using SunsetSystems.Entities.Characters;
using SunsetSystems.Inventory.Data;

namespace SunsetSystems.Inventory
{
    public interface IInventoryManager
    {
        public void AddCoterieMemberEquipment(string creatureID, CreatureData creatureData);

        public bool TryGetEquipmentData(string characterKey, out EquipmentData data);

        public bool TryEquipItemInSlot(string characterID, string slotID, EquipableItem item);

        public bool TryEquipItem(string characterKey, EquipableItem item);

        public bool TryUnequipItemFromSlot(string characterID, string slotID);

        public void TransferItem(ItemStorage from, ItemStorage to, InventoryEntry item);

        public void AddMoney(int value);

        public bool TryRemoveMoney(int value);

        public int GetMoney();

        public void SetMoney(int value);
    }
}
