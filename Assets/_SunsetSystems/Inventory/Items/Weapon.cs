using Sirenix.OdinInspector;
using SunsetSystems.Abilities;
using SunsetSystems.Combat;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SunsetSystems.Inventory.Data
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Sunset Inventory/Items/Weapon")]
    public class Weapon : EquipableItem, IWeapon, IAbilitySource
    {
        [field: SerializeField, BoxGroup("Weapon")]
        public AssetReferenceGameObject EquippedInstanceAsset { get; private set; }
        [field: SerializeField, BoxGroup("Weapon")]
        public int DamageModifier { get; private set; } = 0;
        [field: SerializeField, BoxGroup("Weapon")]
        public DamageType DamageType { get; private set; }
        [field: SerializeField, BoxGroup("Weapon")]
        public AbilityRange WeaponType { get; private set; }
        [field: SerializeField, ShowIf("@this.WeaponType == SunsetSystems.Inventory.AbilityRange.Ranged"), BoxGroup("Weapon")]
        public int MaxAmmo { get; private set; } = 3;
        [field: SerializeField, BoxGroup("Weapon")]
        public AttributeType AssociatedAttribute { get; private set; }
        [field: SerializeField, BoxGroup("Weapon")]
        public SkillType AssociatedSkill { get; private set; }

        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used in ShowIf Attribute")]
        private bool ShowRange => WeaponType == AbilityRange.Ranged;
        [SerializeField, ShowIf("ShowRange"), BoxGroup("Weapon")]
        protected int optimalRange = 0, rangeFalloff = 0;
        [SerializeField, BoxGroup("Weapon")]
        protected List<IAbilityConfig> _grantedAbilities = new();

        protected override void Awake()
        {
            base.Awake();
            ItemCategory = ItemCategory.WEAPON;
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            ItemCategory = ItemCategory.WEAPON;
        }

        public bool GetWeaponUsesAmmo()
        {
            return WeaponType == AbilityRange.Ranged && MaxAmmo > 0;
        }

        public RangeData GetRangeData()
        {
            RangeData data = new()
            {
                OptimalRange = optimalRange,
                MaxRange = optimalRange + (2 * rangeFalloff),
                ShortRange = optimalRange - rangeFalloff
            };
            return data;
        }

        public DamageData GetDamageData()
        {
            DamageData data = new()
            {
                DamageModifier = DamageModifier,
                DamageType = DamageType
            };
            return data;
        }

        public IEnumerable<IAbilityConfig> GetAbilities()
        {
            return _grantedAbilities;
        }
    }
}
