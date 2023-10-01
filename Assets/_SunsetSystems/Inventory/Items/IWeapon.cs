using SunsetSystems.Inventory.Data;
using UnityEngine.AddressableAssets;

namespace SunsetSystems.Inventory
{
    public interface IWeapon : IEquipableItem
    {
        WeaponType WeaponType { get; }

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
}
