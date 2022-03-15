﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Apex.AI.Components;
using Entities.Characters.Data;
using Entities.Characters.Actions;

namespace Entities.Characters
{
    [RequireComponent(typeof(NavMeshAgent)),
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

        [SerializeField]
        protected NavMeshAgent agent;
        [SerializeField]
        protected Inventory inventory;
        [SerializeField, ReadOnly]
        protected GridElement _currentGridPosition;
        [ExposeProperty]
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

        public CreatureData Data { get => GetComponent<CreatureData>(); }

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

        public abstract void Move(Vector3 moveTarget);
        public abstract void Move(GridElement moveTarget);
        public abstract void Attack(Creature target);

        public void ForceCreatureToPosition(Vector3 position)
        {
            ClearAllActions();
            Debug.LogWarning("Forcing creature to position: " + position);
            agent.Warp(position);
        }

        protected virtual void Start()
        {
            if (!inventory)
                inventory = ScriptableObject.CreateInstance(typeof(Inventory)) as Inventory;
            agent = GetComponent<NavMeshAgent>();
            ActionQueue.Enqueue(new Idle(this));
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
            Debug.Log("Rotating towards target " + dot);
            return dot >= 0.999f || dot <= -0.999f;
        }

        public IEnumerator FaceTarget(Transform target)
        {
            yield return new WaitUntil(() => RotateTowardsTarget(target));
            StopCoroutine(FaceTarget(target));
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
                //Debug.Log("Action finished!\n" + ActionQueue.Peek());
                ActionQueue.Dequeue();
                if (ActionQueue.Count == 0)
                    ActionQueue.Enqueue(new Idle(this));
                ActionQueue.Peek().Begin();
            }
        }

        public EntityAction PeekActionFromQueue()
        {
            return ActionQueue.Peek();
        }

        public bool HasActionsInQueue()
        {
            return !ActionQueue.Peek().GetType().Equals(typeof(Idle)) || ActionQueue.Count > 1;
        }

        private void OnDrawGizmos()
        {
            float movementRange = GetComponent<StatsManager>().GetCombatSpeed();
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, movementRange);
        }

        public Inventory GetInventory()
        {
            return inventory;
        }

        public CreatureUIData GetCreatureUIData()
        {
            HealthData healthData = GetComponent<StatsManager>().GetHealthData();
            CreatureUIData.CreatureDataBuilder builder = new CreatureUIData.CreatureDataBuilder(Data.FullName,
                Data.Portrait,
                healthData,
                0);
            return builder.Create();
        }
    } 
}
