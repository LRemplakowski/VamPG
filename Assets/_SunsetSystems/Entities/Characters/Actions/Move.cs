using SunsetSystems.Entities.Characters.Actions.Conditions;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace SunsetSystems.Entities.Characters.Actions
{
    public class Move : EntityAction
    {
        private readonly NavMeshAgent navMeshAgent;
        private readonly NavMeshObstacle navMeshObstacle;
        private Vector3 destination;
        public delegate void OnMovementFinished(Creature who);
        public static OnMovementFinished onMovementFinished;
        public delegate void OnMovementStarted(Creature who);
        public static OnMovementStarted onMovementStarted;
        private float stoppingDistance;
        private Transform rotationTarget;
        private Task rotationTask;

        protected override Creature Owner
        {
            get;
            set;
        }

        public Move(Creature owner, Vector3 destination, float stoppingDistance)
        {
            this.Owner = owner;
            this.navMeshAgent = owner.GetComponent<NavMeshAgent>();
            this.navMeshObstacle = owner.GetComponent<NavMeshObstacle>();
            conditions.Add(new Destination(navMeshAgent));
            this.destination = destination;
            this.stoppingDistance = stoppingDistance;
        }

        public Move(Creature owner, Vector3 destination, Transform rotationTarget) : this(owner, destination, 0f)
        {
            this.rotationTarget = rotationTarget;
            rotationTask = new Task(RotateToTarget);
            conditions.Add(new FaceTargetCondition(rotationTask));
        }

        private async void RotateToTarget()
        {
            await Owner.FaceTarget(rotationTarget);
        }

        public override void Abort()
        {
            navMeshAgent.velocity = Vector3.zero;
            navMeshAgent.isStopped = true;
            navMeshAgent.enabled = false;
            navMeshObstacle.enabled = true;
            navMeshAgent.stoppingDistance = 0f;
            if (onMovementFinished != null)
                onMovementFinished.Invoke(this.Owner);
        }

        public override void Begin()
        {
            navMeshObstacle.enabled = false;
            navMeshAgent.enabled = true;
            navMeshAgent.ResetPath();
            navMeshAgent.SetDestination(destination);
            navMeshAgent.isStopped = false;
            navMeshAgent.stoppingDistance = stoppingDistance;
            if (onMovementStarted != null)
                onMovementStarted.Invoke(this.Owner);
        }

        public override bool IsFinished()
        {
            bool finished = base.IsFinished();
            if (rotationTarget != null && conditions.Any(c => (c is Destination d) && d.IsMet()) && rotationTask.Status == TaskStatus.Created)
            {
                rotationTask.Start();
            }
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
