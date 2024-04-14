using CleverCrow.Fluid.UniqueIds;
using Sirenix.OdinInspector;
using SunsetSystems.Persistence;
using System;
using UnityEngine;
using SunsetSystems.Entities.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SunsetSystems.Entities
{
    [RequireComponent(typeof(UniqueId))]
    [RequireComponent(typeof(CachedReferenceManager))]
    public class PersistentEntity : Entity, IPersistentObject
    {
        [SerializeField, ReadOnly]
        private UniqueId _unique;
        [SerializeField]
        private bool _enablePersistence = true;
        public string PersistenceID => _unique.Id;
        public override string ID => PersistenceID;
        public string GameObjectName => gameObject.name;

        [field: SerializeField]
        public List<IPersistentComponent> PersistentComponents { get; private set; } = new();

        [SerializeField, Required]
        protected IEntityReferences _references;
        public override IEntityReferences References
        {
            get
            {
                if (_references == null)
                    _references = GetComponent<IEntityReferences>();
                return _references;
            }
        }

        protected virtual void Awake()
        {
            
        }

        protected virtual void Start()
        {
            if (_enablePersistence)
            {
                if (ScenePersistenceManager.Instance != null)
                    ScenePersistenceManager.Instance.Register(this);
                else
                    Debug.LogError($"Persistent entity {gameObject.name} found no instance of ScenePersistenceManager! Entity will not persist.");
            }
        }

        protected virtual void OnDestroy()
        {
            ScenePersistenceManager.Instance?.Unregister(this);
        }

        protected virtual void OnValidate()
        {
            if (_unique == null)
                _unique = GetComponent<UniqueId>();
            List<IPersistentComponent> cachedComponents = new(PersistentComponents);
            cachedComponents.AddRange(GetComponents<IPersistentComponent>());
            PersistentComponents.Clear();
            PersistentComponents.AddRange(cachedComponents.Distinct());
        }

        public virtual object GetPersistenceData()
        {
            PersistenceData data = new();
            data.GameObjectActive = gameObject.activeSelf;
            if (PersistentComponents.Count > 0)
            {
                data.PersistentComponentData = new();
                foreach (IPersistentComponent persistentComponent in PersistentComponents)
                {
                    data.PersistentComponentData[persistentComponent.ComponentID] = persistentComponent.GetComponentPersistenceData();
                }
            }
            return data;
        }

        public virtual void InjectPersistenceData(object data)
        {
            if (data is not PersistenceData saveData)
                return;
            gameObject.SetActive(saveData.GameObjectActive);
            if (saveData.PersistentComponentData != null)
            {
                foreach (IPersistentComponent component in PersistentComponents)
                {
                    if (saveData.PersistentComponentData.TryGetValue(component.ComponentID, out object componentData))
                        component.InjectComponentPersistenceData(componentData);
                }
            }
        }

        [Serializable]
        protected class PersistenceData
        {
            public bool GameObjectActive;
            public Dictionary<string, object> PersistentComponentData;

            public PersistenceData()
            {

            }
        }
    }
}
