using SunsetSystems.Inventory;
using System.Collections.Generic;
using SunsetSystems.Entities;
using UnityEngine;

namespace SunsetSystems.Entities.Interfaces
{
    public interface ICombatant
    {
        IEntityReferences References { get; }

        IWeapon CurrentWeapon { get; }
        IWeapon PrimaryWeapon { get; }
        IWeapon SecondaryWeapon { get; }

        Vector3 AimingOrigin { get; }

        bool IsInCover { get; }
        IList<Cover> CurrentCoverSources { get; }

        bool TakeDamage(int amount);

        int GetAttributeValue(AttributeType attributeType);
    }
}
