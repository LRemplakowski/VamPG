using UnityEngine;

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

        private void OnDestroy()
        {
            IInitialized.UnregisterInitialization(this);
        }
    }
}
