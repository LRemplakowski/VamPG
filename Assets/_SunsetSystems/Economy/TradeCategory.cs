using System;

namespace SunsetSystems.Economy
{
    [Flags]
    public enum TradeCategory
    {
        Item = 1 << 1,
    }
}