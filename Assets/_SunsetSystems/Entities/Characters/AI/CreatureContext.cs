using Apex.AI;
using System.Collections.Generic;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Entities;
using SunsetSystems.Game;
using SunsetSystems.Combat;
using UnityEngine;
using SunsetSystems.Entities.Interfaces;
using SunsetSystems.Entities.Characters.Interfaces;
using System.Linq;
using SunsetSystems.Combat.Grid;
using SunsetSystems.Entities.Characters.Actions;

public sealed class CreatureContext : IAIContext
{
    private readonly CombatManager combatManager;

    public CreatureContext(ICombatant owner, CombatManager combatManager)
    {
        Owner = owner;
        this.combatManager = combatManager;
    }

    public ICombatant Owner { get; private set; }
    public IActionPerformer ActionPerformer => Owner;

    public ICombatant CurrentTarget { get; set; }

    public IGridCell CurrentMoveTarget { get; set; }

    public List<ICover> CoverSourcesInCombatGrid => combatManager.CurrentEncounter.MyGrid.CachedCoverSources.ToList();

    public bool IsInCombat => GameManager.IsCurrentState(GameState.Combat);

    public bool IsPlayerControlled => Owner.Faction is Faction.PlayerControlled;

    //public bool HasMoved => Behaviour.HasMoved;
    //public bool HasActed => Behaviour.HasActed;

    //public CombatBehaviour Behaviour => Owner.CombatBehaviour;

    //public StatsManager StatsManager => Owner.StatsManager;

    //public StatsData Stats => Owner.Data.Stats;

   // public List<GridElement> PositionsInRange => combatManager.CurrentEncounter.MyGrid.GetElementsInRangeOfActor(Owner);

    public List<ICombatant> OtherCombatants => CombatManager.Instance.Actors.FindAll(c => !c.Equals(Owner));

    public List<ICombatant> PlayerControlledCombatants => CombatManager.Instance.Actors.FindAll(c => c.Faction is Faction.PlayerControlled);

    public List<ICombatant> FriendlyCombatants => CombatManager.Instance
        .Actors
        .FindAll(c => c.Faction is Faction.Friendly);

    public List<ICombatant> EnemyCombatants => CombatManager.Instance
        .Actors
        .FindAll(c => c.Faction is Faction.Hostile);
}
