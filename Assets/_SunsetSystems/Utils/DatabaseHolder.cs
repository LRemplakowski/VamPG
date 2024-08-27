using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SunsetSystems.Core.Database
{
    public class DatabaseHolder : SerializedMonoBehaviour
    {
        public static DatabaseHolder Instance { get; private set; }
        [SerializeField, SerializeReference]
        private List<ScriptableObject> _databases;
        [ShowInInspector, ReadOnly]
        private Dictionary<System.Type, object> _databaseTypesDictionary = new();

        private void Awake()
        {
            _databaseTypesDictionary = new();
            _databases.ForEach(d => Debug.Log($"Initializing database {d}"));
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            foreach (var so in _databases)
            {
                _databaseTypesDictionary[so.GetType()] = so; 
            }
        }

        public T GetDatabase<T>()
        {
            if (_databaseTypesDictionary.TryGetValue(typeof(T), out var value) && value is T database)
                return database;
            return default;
        }
    }
}
