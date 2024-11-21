using SunsetSystems.Combat;
using SunsetSystems.Inventory.Data;
using System;
using UnityEngine.AddressableAssets;

namespace SunsetSystems.Inventory
{
    public interface IWeapon : IEquipableItem
    {
        AbilityRange WeaponType { get; }
        WeaponFireMode FireMode { get; }
        int MaxAmmo { get; }

        bool GetWeaponUsesAmmo();
        CombatActionType GetWeaponCombatActions();
        RangeData GetRangeData();
        DamageData GetDamageData();

        AssetReferenceGameObject EquippedInstanceAsset { get; }
    }

    public struct RangeData
    {
        public int ShortRange, OptimalRange, MaxRange, RangeFalloff;
    }

    public struct DamageData
    {
        public int DamageModifier;
        public DamageType DamageType;
    }

    public enum DamageType
    {
        Slashing, Piercing, Bludgeoning, Supernatural, Fire
    }

    [Flags]
    public enum WeaponFireMode
    {
        Single = 1 << 0, 
        Burst = 1 << 1, 
        Auto = 1 << 2
    }

    public enum AbilityRange
    {
        Melee,
        Ranged,
    }
}
