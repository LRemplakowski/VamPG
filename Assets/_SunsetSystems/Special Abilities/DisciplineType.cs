using System;

[Flags]
public enum DisciplineType
{
    Animalism = 1,
    Auspex = 1 << 1,
    BloodSorcery = 1 << 2,
    Celerity = 1 << 3,
    Dominate = 1 << 4,
    Fortitude = 1 << 5,
    Obfuscate = 1 << 6,
    Oblivion = 1 << 7,
    Potence = 1 << 8,
    Presence = 1 << 9,
    Protean = 1 << 10,
    Athletics = 1 << 11,
    Firearms = 1 << 12,
    Melee = 1 << 13,
    Streetwise = 1 << 14,
    Insight = 1 << 15,
    Medicine = 1 << 16,
    Larceny = 1 << 17,
    Intimidation = 1 << 18,
    Invalid = 0
}