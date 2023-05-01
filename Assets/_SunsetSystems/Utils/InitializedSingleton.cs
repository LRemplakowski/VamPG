using UnityEngine;
using UnityEngine.Rendering;

namespace SunsetSystems.Utils
{
    public abstract class InitializedSingleton<T> : Singleton<T>, IInitialized where T : Component
    {
        public abstract void Initialize();

        public abstract void LateInitialize();

        protected override void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                DontDestroyOnLoad(this.gameObject);
                IInitialized.RegisterInitialization(this);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            if (instance == this)
                IInitialized.UnregisterInitialization(this);
        }
    }
}
