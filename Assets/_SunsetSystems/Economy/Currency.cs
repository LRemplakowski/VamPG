using System;

namespace SunsetSystems.Economy
{
    [Flags]
    public enum Currency
    {
        Money = 1 << 1, 
        Boon = 1 << 2,
    }
}
