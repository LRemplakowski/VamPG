using SunsetSystems.Combat;
using SunsetSystems.Combat.Grid;
using SunsetSystems.Entities.Characters.Actions;
using SunsetSystems.Entities.Interfaces;
using SunsetSystems.Inventory;
using System.Collections.Generic;

namespace SunsetSystems.AI
{
    public interface IDecisionContext
    {
        ICombatant Owner { get; }
        IActionPerformer ActionPerformer { get; }
        GridManager GridManager { get; }

        List<ICombatant> FriendlyCombatants { get; }
        List<ICombatant> HostileCombatants { get; }

        bool CanMove { get; }
        bool CanAct { get; }
        bool IsMyTurn { get; }

        WeaponType CurrentWeaponType { get;  }
    }
}
