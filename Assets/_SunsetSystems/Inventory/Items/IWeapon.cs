using SunsetSystems.Combat;
using SunsetSystems.Inventory.Data;
using System;
using UnityEngine.AddressableAssets;

namespace SunsetSystems.Inventory
{
    public interface IWeapon : IEquipableItem
    {
        WeaponType WeaponType { get; }
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
        public int shortRange, optimalRange, maxRange, rangeFalloff;
    }

    public struct DamageData
    {
        public int damageModifier;
        public DamageType damageType;
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

    public enum WeaponType
    {
        Melee, Ranged
    }
}
