using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Move : Action
{
    private NavMeshAgent navMeshAgent;
    private Vector3 destination;

    public Move(Creature owner, Vector3 destination)
    {
        this.navMeshAgent = owner.GetComponent<NavMeshAgent>();
        conditions.Add(new Destination(owner));
        this.destination = destination;
    }
    public override void Begin()
    {
        navMeshAgent.SetDestination(destination);
    }
}
