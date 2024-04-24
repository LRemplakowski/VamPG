using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SunsetSystems.Utils.Database
{
    public class DatabaseHolder : SerializedMonoBehaviour
    {
        private static DatabaseHolder _instance;
        [SerializeField, SerializeReference]
        private List<ScriptableObject> _databases;

        private void Awake()
        {
            _databases.ForEach(d => Debug.Log($"Initializing database {d}"));
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
