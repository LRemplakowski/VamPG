using SunsetSystems.Inventory;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Entities.Interfaces
{
    public interface ICombatant : IEntity
    {
        IWeapon CurrentWeapon { get; }
        IWeapon PrimaryWeapon { get; }
        IWeapon SecondaryWeapon { get; }

        Vector3 AimingOrigin { get; }

        bool IsInCover { get; }
        IList<Cover> CurrentCoverSources { get; }

        int MovementRange { get; }

        bool TakeDamage(int amount);

        int GetAttributeValue(AttributeType attributeType);
    }
}
