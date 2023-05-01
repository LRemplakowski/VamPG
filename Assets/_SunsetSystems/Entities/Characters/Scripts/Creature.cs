using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Apex.AI.Components;
using Entities.Characters.Data;
using SunsetSystems.Entities.Characters.Actions;
using System.Threading.Tasks;
using NaughtyAttributes;
using UMA.CharacterSystem;
using Redcode.Awaiting;
using UnityEngine.Animations.Rigging;
using SunsetSystems.Animation;
using SunsetSystems.Spellbook;

namespace SunsetSystems.Entities.Characters
{
    [RequireComponent(typeof(NavMeshAgent)),
    RequireComponent(typeof(NavMeshObstacle)),
    RequireComponent(typeof(StatsManager)),
    RequireComponent(typeof(CombatBehaviour)),
    RequireComponent(typeof(CreatureAnimationController)),
    RequireComponent(typeof(Rigidbody)),
    RequireComponent(typeof(CapsuleCollider)),
    RequireComponent(typeof(Animator)),
    RequireComponent(typeof(DynamicCharacterAvatar)),
    RequireComponent(typeof(StatsManager)),
    RequireComponent(typeof(UtilityAIComponent)),
    RequireComponent(typeof(CapsuleCollider)),
    RequireComponent(typeof(WardrobeManager)),
    RequireComponent(typeof(RigBuilder)),
    RequireComponent(typeof(SpellbookManager))]
    public abstract class Creature : PersistentEntity
    {
        private const float LOOK_TOWARDS_ROTATION_SPEED = 5.0f;

        [Button("Rebuild Creature")]
        private void RebuildCreature()
        {
            if (_config == null)
            {
                Debug.LogError("Failed to rebuild creature! There is no Config assigned to Creature component!");
                return;
            }
            _data = new(_config);
            CreatureInitializer.InitializeCreature(this);
        }

        [SerializeField]
        private CreatureData _data;
        public CreatureData Data => _data;

        [SerializeField]
        private CreatureConfig _config;
        [field: SerializeField]
        public NavMeshAgent Agent { get; protected set; }

        [field: SerializeField]
        public NavMeshObstacle NavMeshObstacle { get; protected set; }

        [field: SerializeField]
        public StatsManager StatsManager { get; protected set; }

        [field: SerializeField]
        public CombatBehaviour CombatBehaviour { get; private set; }

        [field: SerializeField]
        public SpellbookManager SpellbookManager { get; private set; }

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
        public bool IsVampire => Data.CreatureType.Equals(CreatureType.Vampire);

        #region Unity messages
        protected override void Awake()
        {
            base.Awake();
            if (!StatsManager)
                StatsManager = GetComponent<StatsManager>();
            if (!Agent)
                Agent = GetComponent<NavMeshAgent>();
            if (!CombatBehaviour)
                CombatBehaviour = GetComponent<CombatBehaviour>();
            if (!NavMeshObstacle)
                NavMeshObstacle = GetComponent<NavMeshObstacle>();
            if (!SpellbookManager)
                SpellbookManager = GetComponent<SpellbookManager>();
            if (Agent)
                Agent.enabled = true;
            if (NavMeshObstacle)
                NavMeshObstacle.enabled = false;
            if (_config)
                _data = new(_config);
            if (StatsManager)
                StatsManager.Initialize(this);
            if (SpellbookManager)
                SpellbookManager.Initialize(this);
        }

        protected override void Start()
        {
            base.Start();
            ActionQueue.Enqueue(new Idle(this));
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Debug.Log($"Destroying creature {gameObject.name}!");
        }

        public void Update()
        {
            if (ActionQueue.Peek().GetType() == typeof(Idle) && ActionQueue.Count > 1)
            {
                ActionQueue.Dequeue();
                ActionQueue.Peek().Begin();
            }
            else if (ActionQueue.Peek().IsFinished())
            {
                ActionQueue.Dequeue();
                if (ActionQueue.Count == 0)
                    ActionQueue.Enqueue(new Idle(this));
                ActionQueue.Peek().Begin();
            }
        }
        #endregion

        #region Actions and control
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
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * LOOK_TOWARDS_ROTATION_SPEED);
            float dot = Quaternion.Dot(transform.rotation, lookRotation);
            return dot >= 0.999f || dot <= -0.999f;
        }

        public async Task FaceTarget(Transform target)
        {
            bool agentState = Agent.enabled;
            bool ObstacleState = NavMeshObstacle.enabled;
            Agent.enabled = true;
            NavMeshObstacle.enabled = false;
            while (!RotateTowardsTarget(target))
            {
                await new WaitForUpdate();
            }
            Agent.enabled = agentState;
            NavMeshObstacle.enabled = ObstacleState;
        }

        public EntityAction PeekActionFromQueue()
        {
            return ActionQueue.Peek();
        }

        public bool HasActionsInQueue()
        {
            return !ActionQueue.Peek().GetType().IsAssignableFrom(typeof(Idle)) || ActionQueue.Count > 1;
        }

        public abstract Move Move(Vector3 moveTarget, float stoppingDistance);
        public abstract Move Move(Vector3 moveTarget);
        public abstract Move Move(GridElement moveTarget);
        public abstract Move MoveAndRotate(Vector3 moveTarget, Transform rotationTarget);
        public abstract Attack Attack(Creature target);
        #endregion

        protected virtual void OnDrawGizmos()
        {
            float movementRange = StatsManager?.GetCombatSpeed() ?? 0f;
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, movementRange);
        }
    }
}
