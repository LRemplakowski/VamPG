using UnityEngine;
using UnityEngine.AI;

namespace SunsetSystems.Entities.Characters.Actions.Conditions
{
    public class KeepWithinStoppingDistanceOfFollowTarget : Condition
    {
        [SerializeField]
        private NavMeshAgent followTarget;
        [SerializeField]
        private NavMeshAgent myAgent;

        public KeepWithinStoppingDistanceOfFollowTarget(NavMeshAgent followTarget, NavMeshAgent myAgent)
        {
            this.followTarget = followTarget;
            this.myAgent = myAgent;
        }

        public override bool IsMet()
        {
            return followTarget.isStopped && Vector3.Distance(followTarget.transform.position, myAgent.transform.position) > myAgent.stoppingDistance;
        }
    }
}
