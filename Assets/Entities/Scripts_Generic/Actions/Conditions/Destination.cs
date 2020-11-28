using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Destination : Condition
{
    private NavMeshAgent agent;

    private const float completionMargin = 0.1f;
    private bool hasPath = false;

    public Destination(NavMeshAgent agent)
    {
        this.agent = agent;
    }
    public override bool IsMet()
    {
        return AtEndOfPath();
    }

    private bool AtEndOfPath()
    {
        hasPath |= agent.hasPath;
        if (hasPath && agent.remainingDistance <= agent.stoppingDistance + completionMargin)
        {
            // Arrived
            hasPath = false;
            return true;
        }

        return false;
    }
}
