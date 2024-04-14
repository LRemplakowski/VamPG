using System.Collections.Generic;
using CleverCrow.Fluid.UniqueIds;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SunsetSystems.Persistence
{
    [RequireComponent(typeof(UniqueId))]
    public class PersistentSceneObject : MonoBehaviour, IPersistentObject
    {
        public string PersistenceID => _unique.Id;
        public string GameObjectName => gameObject.name;

        [SerializeField, ReadOnly]
        private UniqueId _unique;
        
        [SerializeField]
        private List<IPersistentComponent> _persistentComponents = new();
        public List<IPersistentComponent> PersistentComponents => _persistentComponents;

        protected virtual void OnValidate()
        {
            if (_unique == null)
                _unique = GetComponent<UniqueId>();
        }

        private void Start()
        {
            ScenePersistenceManager.Instance?.Register(this);
        }

        private void OnDestroy()
        {
            ScenePersistenceManager.Instance?.Unregister(this);
        }

        public virtual object GetPersistenceData()
        {
            var result = new PersistentObjectData()
            {
                ActiveState = gameObject.activeSelf
            };
            if (PersistentComponents.Count > 0)
            {
                result.PersistentComponentData = new();
                foreach (IPersistentComponent persistentComponent in PersistentComponents)
                {
                    result.PersistentComponentData[persistentComponent.ComponentID] = persistentComponent.GetComponentPersistenceData();
                }
            }
            return result;
        }

        public virtual void InjectPersistenceData(object data)
        {
            if (data is not PersistentObjectData saveData)
                return;
            gameObject.SetActive(saveData.ActiveState);
            if (saveData.PersistentComponentData != null)
            {
                foreach (IPersistentComponent component in PersistentComponents)
                {
                    if (saveData.PersistentComponentData.TryGetValue(component.ComponentID, out object componentData))
                        component.InjectComponentPersistenceData(componentData);
                }
            }
        }

        protected class PersistentObjectData
        {
            public bool ActiveState;
            public Dictionary<string, object> PersistentComponentData;
        }
    }
}
