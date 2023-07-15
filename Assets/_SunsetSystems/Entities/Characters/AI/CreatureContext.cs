using Apex.AI;
using System.Collections.Generic;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Entities.Cover;
using SunsetSystems.Game;
using SunsetSystems.Combat;
using UnityEngine;
using SunsetSystems.Entities.Data;

public sealed class CreatureContext : IAIContext
{
    private readonly CombatManager combatManager;

    public CreatureContext(Creature owner, CombatManager combatManager)
    {
        Owner = owner;
        this.combatManager = combatManager;
    }

    public Creature Owner { get; private set; }

    public Creature CurrentTarget { get; set; }

    public GridElement CurrentMoveTarget { get; set; }

    public List<Cover> CoverSourcesInCombatGrid => combatManager.CurrentEncounter.MyGrid.CoverSourcesInGrid;

    public bool IsInCombat => GameManager.IsCurrentState(GameState.Combat);

    public bool IsPlayerControlled => Owner is PlayerControlledCharacter;

    public bool HasMoved => Behaviour.HasMoved;
    public bool HasActed => Behaviour.HasActed;

    public CombatBehaviour Behaviour => Owner.CombatBehaviour;

    public StatsManager StatsManager => Owner.StatsManager;

    public StatsData Stats => Owner.Data.Stats;

   // public List<GridElement> PositionsInRange => combatManager.CurrentEncounter.MyGrid.GetElementsInRangeOfActor(Owner);

    public List<Creature> OtherCombatants => CombatManager.Instance.Actors.FindAll(c => !c.Equals(Owner));

    public List<Creature> PlayerControlledCombatants => CombatManager.Instance.Actors.FindAll(c => c is PlayerControlledCharacter);

    public List<Creature> FriendlyCombatants => CombatManager.Instance
        .Actors
        .FindAll(c => c.Data.Faction is Faction.Friendly);

    public List<Creature> EnemyCombatants => CombatManager.Instance
        .Actors
        .FindAll(c => c.Data.Faction is Faction.Hostile);
}
