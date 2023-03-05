using Redcode.Awaiting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SunsetSystems.Utils
{
    /// <summary>
    /// Interface for initialization of MonoBehaviours that persist across multiple scenes.
    /// Make sure to call IInitialized.RegisterInitialization(this) in Awake or OnEnable method.
    /// Make sure to call IInitialized.UnregisterInitialization(this) in OnDisable or OnDestroy method.
    /// </summary>
    public interface IInitialized
    {
        private static readonly List<IInitialized> InitializablesCache = new();

        public static async Task InitializeObjectsAsync()
        {
            List<Task> initialization = new();
            foreach (IInitialized initialized in InitializablesCache)
            {
                initialization.Add(initialized.InitializeAsync(initialized.Initialize));
            }
            await Task.WhenAll(initialization);
        }

        public static async Task LateInitializeObjectsAsync()
        {
            List<Task> initialization = new();
            foreach (IInitialized initialized in InitializablesCache)
            {
                initialization.Add(initialized.InitializeAsync(initialized.LateInitialize));
            }
            await Task.WhenAll(initialization);
        }

        protected static void RegisterInitialization(IInitialized initializable)
        {
            InitializablesCache.Add(initializable);
        }

        protected static void UnregisterInitialization(IInitialized initializable)
        {
            InitializablesCache.Remove(initializable);
        }

        private async Task InitializeAsync(Action initialization)
        {
            await Task.Run(async () =>
            {
                await new WaitForUpdate();
                initialization.Invoke();
            });
        }

        /// <summary>
        /// Initialize is performed before the event OnAfterSceneLoad is called.
        /// </summary>
        public void Initialize();

        /// <summary>
        /// Late Initialize is performed after the event OnAfterSceneLoad is called.
        /// </summary>
        public void LateInitialize();
    }
}
