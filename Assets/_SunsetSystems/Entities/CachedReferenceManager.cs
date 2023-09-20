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
        public Transform Transform => transform;
        public GameObject GameObject => gameObject;

        private readonly Dictionary<Type, Component> cachedReferences = new();

        public new T GetComponent<T>() where T : Component
        {
            if (cachedReferences.TryGetValue(typeof(T), out Component value))
            {
                return value as T;
            }
            else
            {
                T component = base.GetComponent<T>();
                cachedReferences[typeof(T)] = component;
                return component;
            }
        }

        public new T GetComponentInChildren<T>() where T : Component
        {
            if (cachedReferences.TryGetValue(typeof(T), out Component value))
            {
                return value as T;
            }
            else
            {
                T component = base.GetComponentInChildren<T>();
                cachedReferences[typeof(T)] = component;
                return component;
            }
        }
    }
}
