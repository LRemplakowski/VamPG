using UnityEngine;
using Apex.AI;
using Apex;
using System.Collections.Generic;

public sealed class CreatureContext : IAIContext
{
    public CreatureContext(Creature owner) 
    {
        Owner = owner;
    }

    public Creature Owner { get; private set; }

    public Creature CurrentTarget { get; set; }

    public bool IsInCombat => StateManager.instance.GetCurrentState().Equals(GameState.Combat);

    public bool IsPlayerControlled => Owner.Faction.Equals(Faction.Player);

    public bool HasMoved { get; set; }
    public bool HasActed { get; set; }

    public CombatBehaviour Behaviour => Owner.GetComponent<CombatBehaviour>();

    public StatsManager StatsManager => Owner.GetComponent<StatsManager>();

    public CharacterStats Stats => StatsManager.Stats;

    public List<Vector3> PositionsInRange => GameManager.GetGridController().GetGridPositionsInRangeOfActor(Owner);

    public List<Creature> OtherCombatants => TurnCombatManager.instance.GetCreaturesInCombat().FindAll(c => !c.Equals(Owner));

    public List<Creature> PlayerControlledCombatants => TurnCombatManager.instance.GetCreaturesInCombat().FindAll(c => c.Faction.Equals(Faction.Player));

    public List<Creature> FriendlyCombatants => TurnCombatManager
        .instance
        .GetCreaturesInCombat()
        .FindAll(c => c.Faction.Equals(Faction.Friendly));

    public List<Creature> EnemyCombatants => TurnCombatManager
        .instance
        .GetCreaturesInCombat()
        .FindAll(c => c.Faction.Equals(Faction.Hostile));
}
