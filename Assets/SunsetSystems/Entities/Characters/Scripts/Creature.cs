using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Apex.AI.Components;
using Entities.Characters.Data;
using Entities.Characters.Actions;
using System.Threading.Tasks;
using SunsetSystems.Utils.Threading;
using SunsetSystems.Equipment;
using NaughtyAttributes;

namespace Entities.Characters
{
    [RequireComponent(typeof(NavMeshAgent)),
    RequireComponent(typeof(NavMeshObstacle)),
    RequireComponent(typeof(StatsManager)),
    RequireComponent(typeof(CombatBehaviour)),
    RequireComponent(typeof(CreatureAnimator)),
    RequireComponent(typeof(Rigidbody)),
    RequireComponent(typeof(CapsuleCollider)),
    RequireComponent(typeof(Animator)),
    RequireComponent(typeof(UMA.CharacterSystem.DynamicCharacterAvatar)),
    RequireComponent(typeof(StatsManager)),
    RequireComponent(typeof(UtilityAIComponent))]
    public abstract class Creature : Entity
    {
        private const float lookTowardsRotationSpeed = 5.0f;

        [field: SerializeField]
        public NavMeshAgent Agent { get; protected set; }

        [field: SerializeField]
        public NavMeshObstacle NavMeshObstacle { get; protected set; }

        [field: SerializeField]
        public StatsManager StatsManager { get; protected set; }

        [field: SerializeField]
        public CreatureData Data { get; protected set; }

        [field: SerializeField]
        public CombatBehaviour CombatBehaviour { get; private set; }

        [SerializeField, ReadOnly]
        protected GridElement _currentGridPosition;
        public GridElement CurrentGridPosition
        {
            get => _currentGridPosition;
            set
            {
                if (_currentGridPosition)
                {
                    _currentGridPosition.Visited = GridElement.Status.NotVisited;
                }
                value.Visited = GridElement.Status.Occupied;
                _currentGridPosition = value;
            }
        }

        private Queue<EntityAction> _actionQueue;
        private Queue<EntityAction> ActionQueue
        {
            get
            {
                if (_actionQueue == null)
                {
                    _actionQueue = new Queue<EntityAction>();
                    AddActionToQueue(new Idle(this));
                }
                return _actionQueue;
            }
        }

        public bool IsAlive => StatsManager.IsAlive();

        private void Awake()
        {
            if (!StatsManager)
                StatsManager = GetComponent<StatsManager>();
            if (!Data)
                Data = GetComponent<CreatureData>();
            if (!Agent)
                Agent = GetComponent<NavMeshAgent>();
            if (!CombatBehaviour)
                CombatBehaviour = GetComponent<CombatBehaviour>();
            if (!NavMeshObstacle)
                NavMeshObstacle = GetComponent<NavMeshObstacle>();
            Agent.enabled = false;
            NavMeshObstacle.enabled = true;
        }

        protected virtual void Start()
        {
            ActionQueue.Enqueue(new Idle(this));
        }

        public void Update()
        {
            if (ActionQueue.Peek().GetType() == typeof(Idle) && ActionQueue.Count > 1)
            {
                ActionQueue.Dequeue();
                ActionQueue.Peek().Begin();
            }
            if (ActionQueue.Peek().IsFinished())
            {
                ActionQueue.Dequeue();
                if (ActionQueue.Count == 0)
                    ActionQueue.Enqueue(new Idle(this));
                ActionQueue.Peek().Begin();
            }
        }

        public void ForceCreatureToPosition(Vector3 position)
        {
            ClearAllActions();
            Debug.LogWarning("Forcing creature to position: " + position);
            Agent.Warp(position);
        }

        public void AddActionToQueue(EntityAction action)
        {
            ActionQueue.Enqueue(action);
        }

        public void ClearAllActions()
        {
            EntityAction currentAction = ActionQueue.Peek();
            if (currentAction != null)
                currentAction.Abort();
            ActionQueue.Clear();
            ActionQueue.Enqueue(new Idle(this));
        }

        public bool RotateTowardsTarget(Transform target)
        {
            if (target == null)
                return true;
            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookTowardsRotationSpeed);
            float dot = Quaternion.Dot(transform.rotation, lookRotation);
            return dot >= 0.999f || dot <= -0.999f;
        }

        public async Task FaceTarget(Transform target)
        {
            while (!RotateTowardsTarget(target))
            {
                await UnityAwaiters.NextFrame();
            }
        }

        public EntityAction PeekActionFromQueue()
        {
            return ActionQueue.Peek();
        }

        public bool HasActionsInQueue()
        {
            return !ActionQueue.Peek().GetType().IsAssignableFrom(typeof(Idle)) || ActionQueue.Count > 1;
        }

        private void OnDrawGizmos()
        {
            float movementRange = GetComponent<StatsManager>().GetCombatSpeed();
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, movementRange);
        }

        public CreatureUIData GetCreatureUIData()
        {
            HealthData healthData = StatsManager.GetHealthData();
            CreatureUIData.CreatureDataBuilder builder = new(Data.FullName,
                Data.Portrait,
                healthData,
                0);
            return builder.Create();
        }

        public abstract void Move(Vector3 moveTarget, float stoppingDistance);
        public abstract void Move(Vector3 moveTarget);
        public abstract void Move(GridElement moveTarget);
        public abstract void Attack(Creature target);
    }
}
