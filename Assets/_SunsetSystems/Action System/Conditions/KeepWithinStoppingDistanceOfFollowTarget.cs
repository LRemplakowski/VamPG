using SunsetSystems.Entities.Characters.Navigation;
using UnityEngine;

namespace SunsetSystems.ActionSystem
{
    public class KeepWithinStoppingDistanceOfFollowTarget : Condition
    {
        [SerializeField]
        private INavigationManager followTarget;
        [SerializeField]
        private INavigationManager myAgent;
        [SerializeField]
        private float distance;

        public KeepWithinStoppingDistanceOfFollowTarget(INavigationManager followTarget, INavigationManager myAgent, float distance)
        {
            this.followTarget = followTarget;
            this.myAgent = myAgent;
            this.distance = distance;
        }

        public override bool IsMet()
        {
            return !followTarget.IsMoving && Vector3.Distance(followTarget.Position, myAgent.Position) <= distance;
        }
    }
}
