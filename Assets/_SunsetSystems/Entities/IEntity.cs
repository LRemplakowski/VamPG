using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Entities.Interfaces
{
    public interface IEntity
    {
        string ID { get; }
        string Name { get; }
        Faction Faction { get; }
        IEntityReferences References { get; }
    }
}
