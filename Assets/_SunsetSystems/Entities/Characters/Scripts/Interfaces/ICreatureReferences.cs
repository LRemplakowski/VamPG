using SunsetSystems.Entities.Characters;
using SunsetSystems.Entities.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Entities.Creatures.Interfaces
{
    public interface ICreatureReferences : IEntityReferences
    {
        CreatureData Data { get; }
    }
}
