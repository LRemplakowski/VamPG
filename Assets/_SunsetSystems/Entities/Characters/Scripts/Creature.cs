using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Redcode.Awaiting;
using Sirenix.OdinInspector;
using SunsetSystems.Entities.Characters.Actions;
using SunsetSystems.Entities.Characters.Interfaces;
using SunsetSystems.Entities.Creatures.Interfaces;
using SunsetSystems.Entities.Data;
using SunsetSystems.Equipment;
using SunsetSystems.Utils.Database;
using UnityEngine;
using UnityEngine.AI;

namespace SunsetSystems.Entities.Characters
{
    public class Creature : PersistentEntity, ICreature
    {
        [Title("Runtime")]
        [ShowInInspector]
        private Queue<EntityAction> _actionQueue = new();
        private Queue<EntityAction> ActionQueue
        {
            get
            {
                if (_actionQueue == null)
                {
                    _actionQueue = new Queue<EntityAction>();
                    _actionQueue.Enqueue(new Idle(this));
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
            if (ActionQueue.Count <= 0)
                ActionQueue.Enqueue(new Idle(this));
            if (ActionQueue.Peek() is Idle && ActionQueue.Count > 1)
            {
                var idle = ActionQueue.Dequeue();
                idle.Abort();
                ActionQueue.Peek().Begin();
            }
            else if (ActionQueue.Peek().EvaluateAction())
            {
                ActionQueue.Dequeue();
                if (ActionQueue.Count == 0)
                    ActionQueue.Enqueue(new Idle(this));
                ActionQueue.Peek().Begin();
            }
        }
        #endregion

        #region ICreature
        public Transform Transform => References.Transform;
        public MonoBehaviour CoroutineRunner => this;
        public new Faction Faction => References.CreatureData.Faction;
        public new ICreatureReferences References
        {
            get
            {
                if (_references is not ICreatureReferences)
                    _references = base.GetComponent<ICreatureReferences>();
                return _references as ICreatureReferences;
            }
        }

        public EntityAction PeekCurrentAction => _actionQueue.Peek();
        public bool HasActionsQueued => PeekCurrentAction is not Idle || _actionQueue.Count > 1;

        public void ForceToPosition(Vector3 position)
        {
            ClearAllActions();
            if (NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, (int)NavMeshAreas.Walkable))
            {
                Debug.Log($"Forcing Creature {gameObject.name} to position: {hit.position}!");
                References.NavMeshAgent.Warp(hit.position);
            }
            else
            {
                Debug.LogError($"Could not force creature {this} to position {position}! Could not find walkable NavMesh!");
            }
        }

        public void ForceToPosition(Transform positionTransform)
        {
            ForceToPosition(positionTransform.position);
        }

        public void ClearAllActions()
        {
            while (ActionQueue.Count > 0)
                ActionQueue.Dequeue().Cleanup();
            ActionQueue.Enqueue(new Idle(this));
        }

        public EntityAction PeekActionFromQueue()
        {
            return ActionQueue.Peek();
        }

        public async Task PerformAction(EntityAction action, bool clearQueue = false)
        {
            if (action.IsPriority || clearQueue)
                ClearAllActions();
            ActionQueue.Enqueue(action);
            await new WaitForUpdate();
            await new WaitUntil(() => action.ActionFinished || action.ActionCanceled);
        }

        [Button]
        public void InjectDataFromTemplate(ICreatureTemplate template)
        {
            gameObject.name = template.FullName;
            References.CreatureData.CopyFromTemplate(template);
            References.StatsManager.CopyFromTemplate(template);
            References.UMAManager.BuildUMAFromTemplate(template);
            References.EquipmentManager.CopyFromTemplate(template);
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode is false)
            {
                UnityEditor.EditorUtility.SetDirty(this);
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
            }
#endif
        }

        [Button]
        public void InjectDataFromTemplate(ICreatureTemplateProvider templateProvider)
        {
            InjectDataFromTemplate(templateProvider.CreatureTemplate);
        }

        public new T GetComponent<T>() where T : Component => References.GetCachedComponent<T>();
        public new T GetComponentInChildren<T>() where T : Component => References.GetCachedComponentInChildren<T>();
        #endregion

        #region ICreatureTemplateProvider
        public ICreatureTemplate CreatureTemplate => new TemplateFromInstance(this);

        [Serializable]
        public class TemplateFromInstance : ICreatureTemplate
        {
            public TemplateFromInstance(ICreature instance)
            {
                DatabaseID = instance.References.CreatureData.DatabaseID;
                ReadableID = instance.References.CreatureData.ReadableID;
                FirstName = instance.References.CreatureData.FirstName;
                LastName = instance.References.CreatureData.LastName;
                Faction = instance.Faction;
                BodyType = instance.References.CreatureData.BodyType;
                CreatureType = instance.References.CreatureData.CreatureType;
                BaseLookWardrobeCollectionID = DatabaseHolder.Instance.GetDatabase<WardrobeCollectionDatabaseFile>().GetAssetID(instance.References.UMAManager.BaseLookWardrobeCollection);
                EquipmentSlotsData = new();
                foreach (var item in instance.References.EquipmentManager.EquipmentSlots)
                {
                    EquipmentSlotsData[item.Key] = item.Value.GetEquippedItem().ReadableID;
                }
                StatsData = new(instance.References.StatsManager.Stats);
            }

            public TemplateFromInstance()
            {

            }

            public string DatabaseID { get; private set; }

            public string ReadableID { get; private set; }

            public string FullName => $"{FirstName} {LastName}".Trim();

            public string FirstName { get; private set; }

            public string LastName { get; private set; }

            public Faction Faction { get; private set; }

            public BodyType BodyType { get; private set; }

            public CreatureType CreatureType { get; private set; }

            public short BaseLookWardrobeCollectionID { get; private set; }

            public Dictionary<EquipmentSlotID, string> EquipmentSlotsData { get; private set; }

            public StatsData StatsData { get; private set; }
        }
        #endregion

        public override object GetPersistenceData()
        {
            return new CreaturePersistenceData(this);
        }

        public override void InjectPersistenceData(object data)
        {
            base.InjectPersistenceData(data);
            if (data is not CreaturePersistenceData creaturePersistenceData)
                return;
            ForceToPosition(creaturePersistenceData.WorldPosition);
        }

        [Serializable]
        private class CreaturePersistenceData : PersistenceData
        {
            public Vector3 WorldPosition;

            public CreaturePersistenceData(Creature creature) : base(creature)
            {
                WorldPosition = creature.References.BodyTransform.position;
            }
        }
    }
}
