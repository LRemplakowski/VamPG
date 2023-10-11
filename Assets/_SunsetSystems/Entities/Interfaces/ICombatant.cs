using SunsetSystems.Combat;
using SunsetSystems.Combat.Grid;
using SunsetSystems.Entities.Characters.Actions;
using SunsetSystems.Inventory;
using SunsetSystems.Spellbook;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace SunsetSystems.Entities.Interfaces
{
    public interface ICombatant : IEntity, IActionPerformer
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
        int SprintRange { get; }
        bool HasMoved { get; }
        bool HasActed { get; }

        bool TakeDamage(int amount);

        int GetAttributeValue(AttributeType attributeType);

        void SignalEndTurn();
    }
}
