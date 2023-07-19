using SunsetSystems.Inventory.Data;

namespace SunsetSystems.Inventory
{
    public interface IWeapon
    {
        WeaponType WeaponType { get; }

        RangeData GetRangeData();
        DamageData GetDamageData();
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
