using Entities.Characters.Actions.Conditions;
using UnityEngine;
using UnityEngine.AI;

namespace Entities.Characters.Actions
{
    public class Move : EntityAction
    {
        private readonly NavMeshAgent navMeshAgent;
        private Vector3 destination;
        public delegate void OnMovementFinished(Creature who);
        public static OnMovementFinished onMovementFinished;
        public delegate void OnMovementStarted(Creature who);
        public static OnMovementStarted onMovementStarted;

        protected override Creature Owner
        {
            get;
            set;
        }

        public Move(NavMeshAgent navMeshAgent, Vector3 destination)
        {
            this.navMeshAgent = navMeshAgent;
            Owner = navMeshAgent.gameObject.GetComponent<Creature>();
            conditions.Add(new Destination(navMeshAgent));
            this.destination = destination;
        }

        public override void Abort()
        {
            navMeshAgent.isStopped = true;
            Debug.Log("Aborting move action!");
            if (onMovementFinished != null)
                onMovementFinished.Invoke(this.Owner);
        }

        public override void Begin()
        {
            Debug.Log("Moving to destination " + conditions[0]);
            navMeshAgent.SetDestination(destination);
            navMeshAgent.isStopped = false;
            if (onMovementStarted != null)
                onMovementStarted.Invoke(this.Owner);
        }

        public override bool IsFinished()
        {
            bool finished = base.IsFinished();
            if (finished)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    } 
}
