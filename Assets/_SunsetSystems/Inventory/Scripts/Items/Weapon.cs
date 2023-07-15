using Sirenix.OdinInspector;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace SunsetSystems.Inventory.Data
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Sunset Inventory/Items/Weapon")]
    public class Weapon : EquipableItem, IWeapon
    {
        public int DamageModifier = 0;
        public DamageType DamageType;
        public WeaponType WeaponType;
        public AttributeType AssociatedAttribute;
        public SkillType AssociatedSkill;

        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used in ShowIf Attribute")]
        private bool ShowRange => WeaponType == WeaponType.Ranged;
        [SerializeField, ShowIf("ShowRange")]
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
            data.damageModifier = DamageModifier;
            data.damageType = DamageType;
            return data;
        }
    }

    public enum WeaponType
    {
        Melee, Ranged
    }
}
