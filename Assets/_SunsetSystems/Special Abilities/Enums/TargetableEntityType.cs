using System;

namespace SunsetSystems.Abilities
{
    [Flags]
    public enum TargetableEntityType
    {
        Mortal = 1 << 0,
        Ghoul = 1 << 1,
        Vampire = 1 << 2,
        Object = 1 << 3,
        Position = 1 << 4,
        Any = int.MaxValue,
    }
}