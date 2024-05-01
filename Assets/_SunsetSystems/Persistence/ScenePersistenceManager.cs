using System;
using System.Collections.Generic;
using System.Linq;
using CleverCrow.Fluid.UniqueIds;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace SunsetSystems.Persistence
{
    [RequireComponent(typeof(UniqueId))]
    public class ScenePersistenceManager : SerializedMonoBehaviour, ISaveable
    {
        [SerializeField, ReadOnly]
        private UniqueId _unique;
        [ReadOnly, SerializeField]
        private HashSet<IPersistentObject> persistentEntitiesSet = new();

        public static ScenePersistenceManager Instance { get; private set; }

        public string DataKey => _unique.Id;

        [Button("Force Validate")]
        private void OnValidate()
        {
            _unique = _unique != null ? _unique : GetComponent<UniqueId>();
            persistentEntitiesSet ??= new();
            var persistentSceneObjects = FindObjectsOfType<MonoBehaviour>(true).OfType<IPersistentObject>();
            persistentEntitiesSet.AddRange(persistentSceneObjects);

        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                _unique ??= GetComponent<UniqueId>();
            }
            else
            {
                Debug.LogWarning($"There is more than one ScenePersistenceManager in scene! Destroying {gameObject.name}!");
                Destroy(this.gameObject);
            }
        }

        private void Start()
        {
            ISaveable.RegisterSaveable(this);
        }

        private void OnDestroy()
        {
            ISaveable.UnregisterSaveable(this);
        }

        public object GetSaveData()
        {
            ScenePersistenceData data = new();
            Dictionary<string, object> persistenceData = new();
            foreach (IPersistentObject persistentEntity in persistentEntitiesSet)
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
            if (data is ScenePersistenceData persistenceData)
            {
                foreach (IPersistentObject persistentEntity in persistentEntitiesSet)
                {
                    if (persistenceData.PersistentData.TryGetValue(persistentEntity.PersistenceID, out object entityData))
                        persistentEntity.InjectPersistenceData(entityData);
                }
            }
        }

        public void Register(IPersistentObject persistentEntity)
        {
            try
            {
                _ = persistentEntity.PersistenceID;
            }
            catch (NullReferenceException e)
            {
                Debug.LogException(e);
            }
            finally
            {
                persistentEntitiesSet.Add(persistentEntity);
            }
        }

        public void Unregister(IPersistentObject persistentEntity)
        {
            persistentEntitiesSet.Remove(persistentEntity);
        }

        [Serializable]
        public class ScenePersistenceData : SaveData
        {
            public Dictionary<string, object> PersistentData = new();
        }
    }
}