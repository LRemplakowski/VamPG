using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using SunsetSystems.Entities.Characters.Actions;
using System.Threading.Tasks;
using Redcode.Awaiting;
using SunsetSystems.Spellbook;
using Sirenix.OdinInspector;
using SunsetSystems.Entities.Characters.Interfaces;
using SunsetSystems.Entities.Interfaces;
using SunsetSystems.Inventory;

namespace SunsetSystems.Entities.Characters
{
    public abstract class Creature : PersistentEntity, ICreature, IEntityReferences
    {
        public IEntityReferences References => this;

        public Transform Transform => this.transform;
        public GameObject GameObject => this.gameObject;

        private const float LOOK_TOWARDS_ROTATION_SPEED = 5.0f;

        [Button("Rebuild Creature")]
        private void RebuildCreature()
        {
            if (_config == null)
            {
                Debug.LogError("Failed to rebuild creature! There is no Config assigned to Creature component!");
                return;
            }
            Data = new(_config);
            CreatureInitializer.InitializeCreature(this);
        }

        [field: SerializeField]
        public CreatureData Data { get; set; }

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
                    _ = PerformAction(new Idle(this));
                }
                return _actionQueue;
            }
        }

        public bool IsAlive => StatsManager.IsAlive();
        public bool IsVampire => Data.CreatureType.Equals(CreatureType.Vampire);

        public IWeapon CurrentWeapon => throw new System.NotImplementedException();

        public IWeapon PrimaryWeapon => throw new System.NotImplementedException();

        public IWeapon SecondaryWeapon => throw new System.NotImplementedException();

        public Vector3 AimingOrigin => throw new System.NotImplementedException();

        public bool IsInCover => throw new System.NotImplementedException();

        public IList<Cover> CurrentCoverSources => throw new System.NotImplementedException();

        #region Unity messages
        protected override void Awake()
        {
            base.Awake();
            if (!Agent)
                Agent = GetComponent<NavMeshAgent>();
            if (!NavMeshObstacle)
                NavMeshObstacle = GetComponent<NavMeshObstacle>();

            if (Agent)
                Agent.enabled = true;
            if (NavMeshObstacle)
                NavMeshObstacle.enabled = false;
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

        public Task PerformAction(EntityAction action)
        {
            if (action.IsPriority)
                ClearAllActions();
            ActionQueue.Enqueue(action);
            return Task.Run(async () =>
            {
                await new WaitForUpdate();
                while (!action.IsFinished())
                    await new WaitForUpdate();
            });
        }

        public bool TakeDamage(int amount)
        {
            throw new System.NotImplementedException();
        }

        public int GetAttributeValue(AttributeType attributeType)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
