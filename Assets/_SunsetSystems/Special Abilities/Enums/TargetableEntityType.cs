using System;

namespace SunsetSystems.Spellbook
{
    [Flags]
    public enum TargetableEntityType
    {
        Mortal = 1 << 0,
        Vampire = 1 << 1,
        Object = 1 << 2,
    }
}