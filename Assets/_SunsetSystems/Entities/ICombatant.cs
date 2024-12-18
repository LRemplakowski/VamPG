using SunsetSystems.ActionSystem;
using SunsetSystems.Entities;
using UltEvents;
using UnityEngine;

namespace SunsetSystems.Combat
{
    public interface ICombatant : IActionPerformer, ITargetable, IContextProvider<ICombatContext>
    {
        UltEvent<ICombatant> OnUsedActionPoint { get; set; }
        UltEvent<ICombatant> OnSpentBloodPoint { get; set; }
        UltEvent<ICombatant> OnDamageTaken { get; set; }

        Vector3 AimingOrigin { get; }
        Vector3 NameplatePosition { get; }

        void SignalEndTurn();
    }
}
