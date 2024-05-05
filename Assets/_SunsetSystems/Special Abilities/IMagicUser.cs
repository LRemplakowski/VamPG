using SunsetSystems.Entities.Creatures.Interfaces;
using SunsetSystems.Entities.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Spellbook
{
    public interface IMagicUser
    {
        ICreatureReferences References { get; }

        IEnumerable<DisciplinePower> KnownPowers { get; }

        bool UsePower(DisciplinePower power, ITargetable target);
    }
}
