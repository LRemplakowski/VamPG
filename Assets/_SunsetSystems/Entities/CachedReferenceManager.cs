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

        public new T GetCachedComponent<T>()
        {
            if (cachedReferences.TryGetValue(typeof(T), out Component value))
            {
                if (value is T result)
                    return result;
            }
            else
            {
                T component = base.GetComponent<T>();
                cachedReferences[typeof(T)] = component as Component;
                return component;
            }
            return default;
        }

        public new T GetCachedComponentInChildren<T>()
        {
            if (cachedReferences.TryGetValue(typeof(T), out Component value))
            {
                if (value is T result)
                    return result;
            }
            else
            {
                T component = base.GetComponentInChildren<T>();
                cachedReferences[typeof(T)] = component as Component;
                return component;
            }
            return default;
        }
    }
}
