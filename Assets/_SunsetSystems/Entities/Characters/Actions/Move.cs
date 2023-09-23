using SunsetSystems.Entities.Characters.Actions.Conditions;
using SunsetSystems.Entities.Characters.Interfaces;
using System;
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
        public static event Action<IActionPerformer> OnMovementFinished;
        public static Action<IActionPerformer> OnMovementStarted;
        private float stoppingDistance;
        private Transform rotationTarget;
        //private Task rotationTask;

        public Move(IActionPerformer owner, Vector3 destination, float stoppingDistance) : base(owner, true)
        {
            this.navMeshAgent = owner.GetComponent<NavMeshAgent>();
            this.navMeshObstacle = owner.GetComponent<NavMeshObstacle>();
            conditions.Add(new Destination(navMeshAgent));
            this.destination = destination;
            this.stoppingDistance = stoppingDistance;
        }

        public Move(IActionPerformer owner, Vector3 destination, Transform rotationTarget) : this(owner, destination, 0f)
        {
            this.rotationTarget = rotationTarget;
            //rotationTask = new Task(RotateToTarget);
        }

        //private async void RotateToTarget()
        //{
        //    //await Owner.FaceTarget(rotationTarget);
        //    await Task.Yield();
        //}

        public override void Abort()
        {
            base.Abort();
            navMeshAgent.velocity = Vector3.zero;
            navMeshAgent.isStopped = true;
            navMeshAgent.enabled = false;
            navMeshObstacle.enabled = true;
            navMeshAgent.stoppingDistance = 0f;
            if (OnMovementFinished != null)
                OnMovementFinished.Invoke(this.Owner);
        }

        public override void Begin()
        {
            navMeshObstacle.enabled = false;
            navMeshAgent.enabled = true;
            navMeshAgent.ResetPath();
            if (navMeshAgent.SetDestination(destination)) 
            {
                navMeshAgent.isStopped = false;
                navMeshAgent.stoppingDistance = stoppingDistance;
                if (OnMovementStarted != null)
                    OnMovementStarted.Invoke(this.Owner);
            }
            else
            {
                Abort();
            }
        }

        public override bool IsFinished()
        {
            bool finished = base.IsFinished();
            if (rotationTarget != null && conditions.Any(c => (c is Destination d) && d.IsMet()))
            {
                //rotationTask.Start();
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
