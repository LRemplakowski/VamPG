using System;
using System.Collections.Generic;
using SunsetSystems.Abilities;
using SunsetSystems.ActionSystem;
using SunsetSystems.Entities;
using SunsetSystems.Equipment;
using UltEvents;
using UnityEngine;

namespace SunsetSystems.Combat
{
    public interface ICombatant : IActionPerformer, ITargetable, IContextProvider<ICombatContext>
    {
        UltEvent<ICombatant> OnChangedGridPosition { get; set; }
        UltEvent<ICombatant> OnUsedActionPoint { get; set; }
        UltEvent<ICombatant> OnSpentBloodPoint { get; set; }
        UltEvent<ICombatant> OnDamageTaken { get; set; }

        bool IsPlayerControlled { get; }

        Vector3 AimingOrigin { get; }
        Vector3 NameplatePosition { get; }

        void SignalEndTurn();
    }
}
