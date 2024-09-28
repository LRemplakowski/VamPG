using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SunsetSystems.Utils.ObjectPooling
{
    public interface IPooledObject
    {
        void ResetObject();
    }

    public interface IObjectPool<T> where T : Component, IPooledObject
    {
        T GetPooledObject();
        T GetPooledObject(Vector3 position, Quaternion rotation);
        void ReturnObject(T returned);
    }

    public abstract class AbstractObjectPool<T> : MonoBehaviour, IObjectPool<T> where T : Component, IPooledObject
    {
        [Title("Object Pooling")]
        [SerializeField, Required]
        private Transform _poolParent;
        [SerializeField, Required]
        private T _objectPrefab;

        private Queue<T> _objectPool = new();

        protected virtual void Awake()
        {
            _objectPool ??= new();
        }

        public T GetPooledObject()
        {
            T result;
            if (_objectPool.Count > 0)
            {
                result = _objectPool.Dequeue();
                result.gameObject.SetActive(true);
            }
            else
            {
                result = Instantiate(_objectPrefab, _poolParent);
            }
            return result;
        }

        public T GetPooledObject(Vector3 position, Quaternion rotation)
        {
            T result;
            if (_objectPool.Count > 0)
            {
                result = _objectPool.Dequeue();
                result.gameObject.SetActive(true);
                result.transform.SetPositionAndRotation(position, rotation);
            }
            else
            {
                result = Instantiate(_objectPrefab, position, rotation, _poolParent);
            }
            return result;
        }

        public void ReturnObject(T returned)
        {
            returned.ResetObject();
            returned.gameObject.SetActive(false);
            returned.transform.SetParent(_poolParent);
            _objectPool.Enqueue(returned);
        }
    }
}
