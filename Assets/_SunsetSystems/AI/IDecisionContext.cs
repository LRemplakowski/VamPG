using SunsetSystems.Combat;
using SunsetSystems.Combat.Grid;
using SunsetSystems.Entities.Characters.Actions;
using SunsetSystems.Entities.Interfaces;
using System.Collections.Generic;

namespace SunsetSystems.AI
{
    public interface IDecisionContext
    {
        ICombatant Owner { get; }
        IActionPerformer ActionPerformer { get; }
        CachedMultiLevelGrid CurrentGrid { get; }

        IEnumerable<ICombatant> FriendlyCombatants { get; }
        IEnumerable<ICombatant> HostileCombatants { get; }

        bool CanMove { get; }
        bool CanAct { get; }
        bool IsMyTurn { get; }
    }
}
