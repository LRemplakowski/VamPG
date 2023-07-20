using Adnc.Utility;
using CleverCrow.Fluid.UniqueIds;
using SunsetSystems.Persistence;
using System;
using UnityEngine;
using SunsetSystems.Entities.Interfaces;

namespace SunsetSystems.Entities
{
    [RequireComponent(typeof(UniqueId))]
    public class PersistentEntity : Entity, IPersistentEntity
    {
        [SerializeField, ReadOnly]
        protected UniqueId _unique;
        [SerializeField]
        private bool _enablePersistence = true;
        public string PersistenceID => _unique?.Id;
        public override string ID => PersistenceID;
        public string GameObjectName => gameObject.name;

        protected virtual void Awake()
        {
            _unique ??= GetComponent<UniqueId>();
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
        }

        public virtual object GetPersistenceData()
        {
            PersistenceData data = new();
            data.GameObjectActive = gameObject.activeSelf;
            return data;
        }

        public virtual void InjectPersistenceData(object data)
        {
            PersistenceData persistenceData = data as PersistenceData;
            gameObject.SetActive(persistenceData.GameObjectActive);
        }

        [Serializable]
        protected class PersistenceData
        {
            public bool GameObjectActive;

            public PersistenceData()
            {

            }
        }
    }
}
