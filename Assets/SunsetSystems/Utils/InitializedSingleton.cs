using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Utils
{
    public abstract class InitializedSingleton<T> : Singleton<T>, IInitialized where T : Component
    {
        public abstract void Initialize();

        protected static HashSet<InitializedSingleton<T>> initializedSingletons = new();
        public static IReadOnlyCollection<InitializedSingleton<T>> InitializedSingletons => initializedSingletons;

        protected override void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                DontDestroyOnLoad(this.gameObject);
                initializedSingletons.Add(this);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        private void OnDestroy()
        {
            if (initializedSingletons.Contains(this))
                initializedSingletons.Remove(this);
        }
    }
}
