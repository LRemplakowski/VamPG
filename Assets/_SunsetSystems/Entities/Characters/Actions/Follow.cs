using System.Collections;
using SunsetSystems.Entities.Characters.Actions.Conditions;
using SunsetSystems.Entities.Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace SunsetSystems.Entities.Characters.Actions
{
    public class Follow : EntityAction
    {
        private readonly NavMeshAgent followTarget;
        private readonly NavMeshAgent myAgent;

        private IEnumerator followCoroutine;
        private bool following;

        public Follow(IActionPerformer owner, IEntity followTarget) : base(owner)
        {
            this.followTarget = followTarget.References.GetCachedComponentInChildren<NavMeshAgent>();
            myAgent = owner.References.NavMeshAgent;
            following = false;
            conditions.Add(new KeepWithinStoppingDistanceOfFollowTarget(this.followTarget, myAgent));
        }

        public override void Abort()
        {
            base.Abort();
            if (followCoroutine != null)
                Owner.CoroutineRunner.StopCoroutine(followCoroutine);
            myAgent.isStopped = true;
            following = false;
        }

        public override void Begin()
        {
            myAgent.isStopped = false;
            following = true;
            followCoroutine = FollowCoroutine();
            Owner.CoroutineRunner.StartCoroutine(followCoroutine);
        }

        private IEnumerator FollowCoroutine()
        {
            while (following)
            {
                myAgent.isStopped = false;
                myAgent.destination = followTarget.transform.position;
                yield return null;
            }
        }
    }
}
