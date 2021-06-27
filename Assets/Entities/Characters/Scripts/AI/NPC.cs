using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPCombatBehaviour))]
public class NPC : Creature
{
    [SerializeField]
    private Faction _faction;
    public Faction Faction
    {
        get => _faction;
        set => _faction = value;
    }

    [SerializeField]
    private Transform _lineTarget;

    public Transform LineTarget
    {
        get => _lineTarget;
        private set => _lineTarget = value;
    }

    public override void Move(Vector3 moveTarget)
    {
        ClearAllActions();
        AddActionToQueue(new Move(agent, moveTarget));
    }

    public override void Move(GridElement moveTarget)
    {
        ClearAllActions();
        CurrentGridPosition = moveTarget;
        AddActionToQueue(new Move(agent, moveTarget.transform.position));
    }

    public override void Attack(Creature target)
    {
        ClearAllActions();
        AddActionToQueue(new Attack(target, this));
    }
}
