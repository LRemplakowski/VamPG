using System;
using System.Collections;
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
            persistentEntitiesSet.RemoveWhere(element => element == null);
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
                if (persistentEntity == null && string.IsNullOrWhiteSpace(persistentEntity.PersistenceID))
                {
                    Debug.LogError($"Tried to save a null entity or entity has a null/whitespace ID!");
                    continue;
                }    
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
                    if (persistentEntity == null || string.IsNullOrWhiteSpace(persistentEntity.PersistenceID))
                    {
                        Debug.LogWarning($"Encountered a null persistent entity or persistent entity with invalid persitence id! {persistentEntity}");
                        continue;
                    }

                    try
                    {
                        InjectPersistentData(persistenceData, persistentEntity);
                    }
                    catch
                    {
                        float retryAfterTime = 10f;
                        Debug.LogException(new InvalidOperationException($"Injecting data into {persistentEntity} failed! Retrying injection in {Mathf.RoundToInt(retryAfterTime)} seconds!"));
                        StartCoroutine(RetryInjectionInTime(retryAfterTime));

                        IEnumerator RetryInjectionInTime(float time)
                        {
                            float timeElapsed = 0f;
                            while (timeElapsed < time)
                            {
                                timeElapsed += Time.deltaTime;
                                yield return null;
                            }
                            Debug.Log($"Retrying injection for object {persistentEntity}!");
                            try
                            {
                                InjectPersistentData(persistenceData, persistentEntity);
                            }
                            catch (Exception e)
                            {
                                Debug.LogError($"Data injection retry for {persistentEntity} failed due to exception {e}! WTF?!");
                            }
                        }
                    }
                }
            }
        }

        private static void InjectPersistentData(ScenePersistenceData persistenceData, IPersistentObject persistentEntity)
        {
            if (persistentEntity.EnablePersistence)
            {
                Debug.Log($"Persistence is disabled for entity {persistentEntity}! Continuing...");
                return;
            }

            if (persistenceData.PersistentData.TryGetValue(persistentEntity.PersistenceID, out object entityData))
            {
                persistentEntity.InjectPersistenceData(entityData);
                Debug.Log($"Successfuly injected persistence data into {persistentEntity}!");
            }
        }

        [Serializable]
        public class ScenePersistenceData : SaveData
        {
            public Dictionary<string, object> PersistentData = new();

            public ScenePersistenceData()
            {
                PersistentData = new();
            }
        }
    }
}