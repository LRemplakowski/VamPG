using CleverCrow.Fluid.UniqueIds;
using NaughtyAttributes;
using SunsetSystems.Entities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Persistence
{
    [RequireComponent(typeof(UniqueId))]
    public class ScenePersistenceManager : MonoBehaviour, ISaveable
    {
        private HashSet<IPersistentEntity> persistentEntitiesSet = new();
        [SerializeField, ReadOnly]
        private int persistentEntitiesCount = 0;
        [SerializeField, ReadOnly]
        private List<PersistentEntity> persistentEntitiesList = new();
        [SerializeField, ReadOnly]
        private UniqueId _unique;

        public static ScenePersistenceManager Instance { get; private set; }

        public string DataKey => _unique.Id;

        public object GetSaveData()
        {
            ScenePersistenceData data = new();
            Dictionary<string, object> persistenceData = new();
            foreach (IPersistentEntity persistentEntity in persistentEntitiesSet)
            {
                if (persistenceData.TryAdd(persistentEntity.PersistenceID, persistentEntity.GetPersistenceData()) == false)
                {
                    Debug.LogError($"Persistence data dictionary already contains key for {persistentEntity}! Key: {persistentEntity.PersistenceID}");
                    continue;
                }
            }
            data.PersistentData = persistenceData;
            if (data.PersistentData == null)
                throw new NullReferenceException("FOO");
            return data;
        }

        public void InjectSaveData(object data)
        {
            ScenePersistenceData savedData = data as ScenePersistenceData;
            foreach (IPersistentEntity persistentEntity in persistentEntitiesSet)
            {
                persistentEntity.InjectPersistenceData(savedData.PersistentData[persistentEntity.PersistenceID]);
            }
        }

        public void Register(IPersistentEntity persistentEntity)
        {
            if (string.IsNullOrWhiteSpace(persistentEntity.PersistenceID))
            {
                Debug.LogError($"Entity {persistentEntity} does not have valid Persistence ID! It will not be saved.");
                return;
            }
            persistentEntitiesSet.Add(persistentEntity);
            persistentEntitiesCount = persistentEntitiesSet.Count;
        }

        public void Unregister(IPersistentEntity persistentEntity)
        {
            persistentEntitiesSet.Remove(persistentEntity);
            persistentEntitiesCount = persistentEntitiesSet.Count;
        }
#if UNITY_EDITOR
        [Button]
        private void UpdateEntitiesInspectorList()
        {
            persistentEntitiesList = new();
            foreach (IPersistentEntity entity in persistentEntitiesSet)
            {
                persistentEntitiesList.Add(entity as PersistentEntity);
            }
        }
#endif

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                _unique ??= GetComponent<UniqueId>();
                ISaveable.RegisterSaveable(this);
                persistentEntitiesSet = new();
            }
            else
            {
                Debug.LogWarning($"There is more than one ScenePersistenceManager in scene! Destroying {gameObject.name}!");
                Destroy(this.gameObject);
            }
        }

        private void OnDestroy()
        {
            ISaveable.UnregisterSaveable(this);
        }

        private void OnValidate()
        {
            _unique ??= GetComponent<UniqueId>();
        }

        [Serializable]
        private class ScenePersistenceData : SaveData
        {
            public Dictionary<string, object> PersistentData;
        }
    }
}
