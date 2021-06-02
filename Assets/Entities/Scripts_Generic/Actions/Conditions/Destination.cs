using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Destination : Condition
{
    private NavMeshAgent agent;

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
        if (hasPath && agent.remainingDistance <= agent.stoppingDistance + ActionConsts.COMPLETION_MARGIN)
        {
            // Arrived
            hasPath = false;
            return true;
        }

        return false;
    }

    public override string ToString()
    {
        string condition = "Type<Destination>:\n" +
            "Distance to target: " + agent.remainingDistance + "\n" +
            "Agent has path? " + agent.hasPath;
        return condition;
    }
}
