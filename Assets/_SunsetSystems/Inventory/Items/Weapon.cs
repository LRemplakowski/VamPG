using Sirenix.OdinInspector;
using SunsetSystems.Combat;
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
        public AbilityRange WeaponType { get; private set; }
        [field: SerializeField, ShowIf("@this.WeaponType == SunsetSystems.Inventory.AbilityRange.Ranged")]
        public WeaponFireMode FireMode { get; private set; }
        [field: SerializeField, ShowIf("@this.WeaponType == SunsetSystems.Inventory.AbilityRange.Ranged")]
        public int MaxAmmo { get; private set; } = 3;
        [SerializeField]
        private bool _overrideWeaponActions = false;
        [SerializeField, ShowIf("_overrideWeaponActions")]
        private CombatActionType _actionsOverride;
        [field: SerializeField]
        public AttributeType AssociatedAttribute { get; private set; }
        [field: SerializeField]
        public SkillType AssociatedSkill { get; private set; }

        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used in ShowIf Attribute")]
        private bool ShowRange => WeaponType == AbilityRange.Ranged;
        [SerializeField, ShowIf("ShowRange")]
        protected int optimalRange = 0, rangeFalloff = 0;

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

        public CombatActionType GetWeaponCombatActions()
        {
            if (_overrideWeaponActions)
                return _actionsOverride;
            CombatActionType result = CombatActionType.None;
            switch (this.WeaponType)
            {
                case AbilityRange.Melee:
                    result |= CombatActionType.MeleeAtk;
                    break;
                case AbilityRange.Ranged:
                    result |= CombatActionType.RangedAtk;
                    result |= CombatActionType.Reload;
                    break;
            }
            return result;
        }
    }
}
