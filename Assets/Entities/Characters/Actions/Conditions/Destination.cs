namespace Entities.Characters.Actions.Conditions
{
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
            Debug.Log("Remaining distance: " + agent.remainingDistance + "; HasPath? " + hasPath);
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
}
