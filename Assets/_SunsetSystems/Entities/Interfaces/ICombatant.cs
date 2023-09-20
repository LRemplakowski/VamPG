using SunsetSystems.Combat;
using SunsetSystems.Combat.Grid;
using SunsetSystems.Inventory;
using SunsetSystems.Spellbook;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Entities.Interfaces
{
    public interface ICombatant : IEntity
    {
        IMagicUser MagicUser { get; }

        bool IsPlayerControlled { get; }
        IWeapon CurrentWeapon { get; }
        IWeapon PrimaryWeapon { get; }
        IWeapon SecondaryWeapon { get; }

        Vector3 AimingOrigin { get; }

        bool IsInCover { get; }
        IList<ICover> CurrentCoverSources { get; }

        int MovementRange { get; }
        bool HasMoved { get; }
        bool HasActed { get; }

        bool TakeDamage(int amount);

        int GetAttributeValue(AttributeType attributeType);

        void MoveToGridPosition(int x, int y, int z);
        void MoveToGridPosition(Vector3Int gridPosition) => MoveToGridPosition(gridPosition.x, gridPosition.y, gridPosition.z);
        void MoveToGridPosition(GridUnitObject gridObject);
        void MoveToGridPosition(GridUnit gridUnit);
    }
}
