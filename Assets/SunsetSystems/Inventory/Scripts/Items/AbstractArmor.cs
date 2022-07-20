using UnityEngine;

namespace SunsetSystems.Inventory.Data
{
    public abstract class AbstractArmor : EquipableItem
    {
        [SerializeField]
        private int _armor = 0;
        public int Armor => _armor;
    }
}
