using Sirenix.OdinInspector;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SunsetSystems.Inventory.Data
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Sunset Inventory/Items/Weapon")]
    public class Weapon : EquipableItem, IWeapon
    {
        [field: SerializeField]
        public AssetReferenceGameObject EquippedInstanceAsset { get; private set; }
        [field: SerializeField]
        public int DamageModifier { get; private set; } = 0;
        [field: SerializeField]
        public DamageType DamageType { get; private set; }
        [field: SerializeField]
        public WeaponType WeaponType { get; private set; }
        [field: SerializeField]
        public AttributeType AssociatedAttribute { get; private set; }
        [field: SerializeField]
        public SkillType AssociatedSkill { get; private set; }

        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used in ShowIf Attribute")]
        private bool ShowRange => WeaponType == WeaponType.Ranged;
        [SerializeField, ShowIf("ShowRange")]
        protected int optimalRange = 0, rangeFalloff = 0;

        private void Awake()
        {
            ItemCategory = ItemCategory.WEAPON;
        }

        private void OnValidate()
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
