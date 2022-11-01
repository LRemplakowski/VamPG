using UnityEngine;

namespace SunsetSystems.Inventory.Data
{
    public abstract class AbstractArmor : EquipableItem, IArmor
    {
        [SerializeField]
        protected int armorRating = 0;

        public int GetArmorRating()
        {
            return armorRating;
        }
    }
}
