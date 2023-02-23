using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Utils.Database
{
    public class DatabaseHolder : MonoBehaviour
    {
        [SerializeField]
        private List<ScriptableObject> _databases;

        private void Awake()
        {
            _databases.ForEach(d => Debug.Log($"Initializing database {d}"));
        }
    }
}
