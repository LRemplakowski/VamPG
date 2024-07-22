using SunsetSystems.Combat;
using SunsetSystems.Combat.Grid;
using SunsetSystems.Entities.Characters.Actions;
using SunsetSystems.Inventory;
using SunsetSystems.Spellbook;
using System.Collections.Generic;
using System.Threading.Tasks;
using UltEvents;
using UnityEngine;

namespace SunsetSystems.Entities.Interfaces
{
    public interface ICombatant : IActionPerformer
    {
        UltEvent<ICombatant> OnChangedGridPosition { get; set; }
        UltEvent<ICombatant> OnUsedActionPoint { get; set; }
        UltEvent<ICombatant> OnSpentBloodPoint { get; set; }
        UltEvent<ICombatant> OnWeaponChanged { get; set; }
        UltEvent<ICombatant> OnDamageTaken { get; set; }

        IMagicUser MagicUser { get; }
        Faction Faction { get; }

        bool IsPlayerControlled { get; }
        IWeapon CurrentWeapon { get; }
        IWeapon PrimaryWeapon { get; }
        IWeapon SecondaryWeapon { get; }

        Vector3 AimingOrigin { get; }
        Vector3 NameplatePosition { get; }

        bool IsInCover { get; }
        bool IsAlive { get; }
        IList<ICover> CurrentCoverSources { get; }

        int MovementRange { get; }
        int SprintRange { get; }
        bool HasMoved { get; }
        bool HasActed { get; }

        bool TakeDamage(int amount);
        int GetAttributeValue(AttributeType attributeType);
        void SignalEndTurn();

        bool MoveToGridPosition(Vector3Int gridPosition);
        bool AttackCreatureUsingCurrentWeapon(ICombatant target);
        bool ReloadCurrentWeapon();


        float PerformAttackAnimation();
        float PerformTakeHitAnimation();
    }
}
