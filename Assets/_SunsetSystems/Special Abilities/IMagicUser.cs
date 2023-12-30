using SunsetSystems.Entities.Creatures.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Spellbook
{
    public interface IMagicUser
    {
        ICreatureReferences References { get; }

        IEnumerable<DisciplinePower> KnownPowers { get; }

        void UsePower(DisciplinePower power, IMagicUser castingActor);
        void UsePowerAfterTargetSelection(DisciplinePower power);
    }
}
