using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Move : EntityAction
{
    private NavMeshAgent navMeshAgent;
    private Vector3 destination;
    public delegate void OnMovementFinished();
    public static OnMovementFinished onMovementFinished;
    public delegate void OnMovementStarted();
    public static OnMovementStarted onMovementStarted;

    public Move(NavMeshAgent navMeshAgent, Vector3 destination)
    {
        this.navMeshAgent = navMeshAgent;
        conditions.Add(new Destination(navMeshAgent));
        this.destination = destination;
    }

    public Move(NavMeshAgent navMeshAgent, GridElement target)
    {
        this.navMeshAgent = navMeshAgent;
        conditions.Add(new Destination(navMeshAgent));
        this.destination = target.transform.position;
        owner.CurrentGridPosition = target;
    }

    public override void Abort()
    {
        navMeshAgent.isStopped = true;
    }

    public override void Begin()
    {
        Debug.Log("Player destination == " + destination);
        navMeshAgent.SetDestination(destination);
        navMeshAgent.isStopped = false;
        if (onMovementStarted != null)
            onMovementStarted.Invoke();
    }

    public override bool IsFinished()
    {
        bool finished = base.IsFinished();
        if (finished)
        {
            if (onMovementFinished != null)
                onMovementFinished.Invoke();
            return true;
        }
        else
        {
            return false;
        }
    }
}
