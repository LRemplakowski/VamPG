using System.Collections.Generic;
using SunsetSystems.Combat;
using SunsetSystems.Entities.Characters;

namespace SunsetSystems.Spellbook
{
    public interface IMagicUser
    {
        ICreatureReferences References { get; }

        IEnumerable<DisciplinePower> KnownPowers { get; }

        bool UsePower(DisciplinePower power, ITargetable target);
    }
}
