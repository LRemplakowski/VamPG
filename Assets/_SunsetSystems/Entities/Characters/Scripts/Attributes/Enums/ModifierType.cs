using System;

[Flags]
public enum ModifierType
{
    ALL = int.MaxValue,
    NONE = 0,
    SURGE = 1,
    PASSIVE = 2,
}
