using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Utils
{
    public abstract class ScriptableObjectSingleton<T> : ScriptableObject where T : ScriptableObjectSingleton<T>
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    T[] all = UnityEngine.Resources.LoadAll<T>("");
                    if (all == null || all.Length <= 0)
                    {
                        throw new System.NullReferenceException("No istance of " + typeof(T).Name + " singleton found!");
                    }
                    else if (all.Length > 1)
                    {
                        Debug.LogWarning("There is more than one istance of asset " + typeof(T).Name);
                    }
                    _instance = all[0];
                }
                return _instance;
            }
        }
    }
}
