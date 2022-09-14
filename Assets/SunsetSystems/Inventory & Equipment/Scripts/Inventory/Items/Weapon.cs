using UnityEngine;

namespace SunsetSystems.Inventory.Data
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Sunset Inventory/Items/Weapon")]
    public class Weapon : EquipableItem, IWeapon
    {
        [SerializeField]
        protected int damageModifier = 0;
        [SerializeField]
        protected DamageType damageType;
        [SerializeField]
        protected int optimalRange = 0, rangeFalloff = 0;

        private void Awake()
        {
            ItemCategory = ItemCategory.WEAPON;
        }

        public RangeData GetRangeData()
        {
            RangeData data = new();
            data.optimalRange = optimalRange;
            data.rangeFalloff = rangeFalloff;
            data.maxRange = optimalRange + (2 * rangeFalloff);
            data.shortRange = optimalRange - rangeFalloff;
            return data;
        }

        public DamageData GetDamageData()
        {
            DamageData data = new();
            data.damageModifier = damageModifier;
            data.damageType = damageType;
            return data;
        }
    }
}
