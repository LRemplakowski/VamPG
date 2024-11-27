using System;
using System.Collections.Generic;
using SunsetSystems.Entities.Characters.Actions;
using SunsetSystems.Equipment;
using SunsetSystems.Inventory;
using SunsetSystems.Abilities;
using UltEvents;
using UnityEngine;

namespace SunsetSystems.Combat
{
    public interface ICombatant : IActionPerformer, ITargetable
    {
        UltEvent<ICombatant> OnChangedGridPosition { get; set; }
        UltEvent<ICombatant> OnUsedActionPoint { get; set; }
        UltEvent<ICombatant> OnSpentBloodPoint { get; set; }
        UltEvent<ICombatant> OnDamageTaken { get; set; }

        IAbilityUser AbilityUser { get; }
        ITargetable Targetable { get; }
        Faction Faction { get; }

        bool IsPlayerControlled { get; }
        IWeaponManager WeaponManager {get;}

        Vector3 AimingOrigin { get; }
        Vector3 NameplatePosition { get; }

        bool IsInCover { get; }
        bool IsAlive { get; }
        IList<ICover> CurrentCoverSources { get; }

        int MovementRange { get; }
        int SprintRange { get; }
        [Obsolete]
        bool HasMoved { get; }
        [Obsolete]
        bool HasActed { get; }

        bool GetCanMove();
        int GetRemainingActionPoints();
        int GetRemainingMovement();

        bool TakeDamage(int amount);
        int GetAttributeValue(AttributeType attributeType);
        void SignalEndTurn();

        bool MoveToGridPosition(Vector3Int gridPosition);
        bool AttackCreatureUsingCurrentWeapon(ICombatant target);
        bool ReloadCurrentWeapon();
        bool IsTargetInRange(ICombatant target);
    }
}
