namespace SunsetSystems.Entities.Characters.Actions.Conditions
{
    using UnityEngine;
    using UnityEngine.AI;

    [System.Serializable]
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
    }
}
