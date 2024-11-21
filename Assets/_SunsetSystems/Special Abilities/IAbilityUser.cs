using System.Collections.Generic;
using SunsetSystems.Combat;
using SunsetSystems.Entities.Characters;

namespace SunsetSystems.Spellbook
{
    public interface IAbilityUser
    {
        ICreatureReferences References { get; }

        IEnumerable<DisciplinePower> KnownPowers { get; }
        IEnumerable<Discipline> KnownDisciplines { get; }

        bool UsePower(DisciplinePower power, ITargetable target);
    }
}
