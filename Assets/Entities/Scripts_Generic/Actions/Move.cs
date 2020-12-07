using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Move : EntityAction
{
    private NavMeshAgent navMeshAgent;
    private Vector3 destination;

    public Move(NavMeshAgent navMeshAgent, Vector3 destination)
    {
        this.navMeshAgent = navMeshAgent;
        conditions.Add(new Destination(navMeshAgent));
        this.destination = destination;
    }

    public override void Abort()
    {
        navMeshAgent.isStopped = true;
    }

    public override void Begin()
    {
        navMeshAgent.SetDestination(destination);
        navMeshAgent.isStopped = false;
    }
}
