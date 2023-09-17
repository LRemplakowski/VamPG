using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using SunsetSystems.Entities.Characters.Actions;
using System.Threading.Tasks;
using Redcode.Awaiting;
using Sirenix.OdinInspector;
using SunsetSystems.Entities.Characters.Interfaces;
using SunsetSystems.Entities.Interfaces;
using SunsetSystems.Inventory;
using SunsetSystems.Entities.Creatures.Interfaces;
using UnityEngine.AddressableAssets;

namespace SunsetSystems.Entities.Characters
{
    public class Creature : PersistentEntity, ICreature, ICombatant, ICreatureTemplateProvider
    {
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

        #region Unity messages
        protected override void Awake()
        {
            base.Awake();
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

        #region ICreature
        public new Faction Faction => References.CreatureData.Faction;
        public new ICreatureReferences References
        {
            get
            {
                if (_references is not ICreatureReferences)
                    _references = GetComponent<ICreatureReferences>();
                return _references as ICreatureReferences;
            }
        }

        public void ForceToPosition(Vector3 position)
        {
            ClearAllActions();
            Debug.Log($"Forcing Creature {gameObject.name} to position: {position}");
            References.GetComponent<NavMeshAgent>().Warp(position);
        }

        public void ClearAllActions()
        {
            EntityAction currentAction = ActionQueue.Peek();
            if (currentAction != null)
                currentAction.Abort();
            ActionQueue.Clear();
            ActionQueue.Enqueue(new Idle(this));
        }

        public EntityAction PeekActionFromQueue()
        {
            return ActionQueue.Peek();
        }

        public bool HasActionsInQueue()
        {
            return !ActionQueue.Peek().GetType().IsAssignableFrom(typeof(Idle)) || ActionQueue.Count > 1;
        }

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

        public void InjectDataFromTemplate(ICreatureTemplate template)
        {
            throw new System.NotImplementedException();
        }
        #endregion

        #region ICombatant
        public IWeapon CurrentWeapon => throw new System.NotImplementedException();
        public IWeapon PrimaryWeapon => throw new System.NotImplementedException();
        public IWeapon SecondaryWeapon => throw new System.NotImplementedException();
        public Vector3 AimingOrigin => throw new System.NotImplementedException();
        public bool IsInCover => throw new System.NotImplementedException();
        public IList<Cover> CurrentCoverSources => throw new System.NotImplementedException();

        public bool TakeDamage(int amount)
        {
            throw new System.NotImplementedException();
        }

        public int GetAttributeValue(AttributeType attributeType)
        {
            throw new System.NotImplementedException();
        }
        #endregion

        #region ICreatureTemplateProvider
        public ICreatureTemplate CreatureTemplate => new TemplateFromInstance(this);

        public int MovementRange => throw new System.NotImplementedException();

        private class TemplateFromInstance : ICreatureTemplate
        {
            public TemplateFromInstance(ICreature instance)
            {

            }

            public string DatabaseID => throw new System.NotImplementedException();

            public string ReadableID => throw new System.NotImplementedException();

            public string FullName => throw new System.NotImplementedException();

            public Faction Faction => throw new System.NotImplementedException();

            public BodyType BodyType => throw new System.NotImplementedException();

            public CreatureType CreatureType => throw new System.NotImplementedException();

            public AssetReferenceSprite PortraitAssetRef => throw new System.NotImplementedException();
        }
        #endregion
    }
}
