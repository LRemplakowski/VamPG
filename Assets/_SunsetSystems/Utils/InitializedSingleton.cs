using SunsetSystems.Utils.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace SunsetSystems.Utils
{
    public abstract class InitializedSingleton<T> : Singleton<T>, IInitialized where T : Component
    {
        public abstract void Initialize();

        protected static HashSet<InitializedSingleton<T>> _initializedSingletons = new();
        public static IReadOnlyCollection<InitializedSingleton<T>> InitializedSingletons => _initializedSingletons;

        protected override void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                DontDestroyOnLoad(this.gameObject);
                _initializedSingletons.Add(this);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        private void OnDestroy()
        {
            if (_initializedSingletons.Contains(this))
                _initializedSingletons.Remove(this);
        }
    }
}
