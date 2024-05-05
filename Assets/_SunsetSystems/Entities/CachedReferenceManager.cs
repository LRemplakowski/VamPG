using Sirenix.OdinInspector;
using SunsetSystems.Entities.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Entities
{
    public class CachedReferenceManager : SerializedMonoBehaviour, IEntityReferences
    {
        public virtual Transform Transform => transform;
        public virtual GameObject GameObject => gameObject;

        private readonly Dictionary<Type, Component> cachedReferences = new();

        public T GetCachedComponent<T>()
        {
            if (cachedReferences.TryGetValue(typeof(T), out Component value))
            {
                if (value is T result)
                    return result;
            }
            else
            {
                T component = GetComponent<T>();
                if (component == null)
                    component = GetCachedComponentInChildren<T>();
                cachedReferences[typeof(T)] = component as Component;
                return component;
            }
            return default;
        }

        public T GetCachedComponentInChildren<T>()
        {
            if (cachedReferences.TryGetValue(typeof(T), out Component value))
            {
                if (value is T result)
                    return result;
            }
            else
            {
                T component = GetComponentInChildren<T>();
                cachedReferences[typeof(T)] = component as Component;
                return component;
            }
            return default;
        }
    }
}
