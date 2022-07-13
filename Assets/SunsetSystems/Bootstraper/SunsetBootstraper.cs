using SunsetSystems.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.Threading;

namespace SunsetSystems.Bootstraper
{
    public class SunsetBootstraper : Singleton<SunsetBootstraper>
    {
        [SerializeField]
        private List<int> bootstrapSceneIndexes;

#if UNITY_EDITOR
        protected async void Start()
        {
            base.Awake();
            foreach (int index in bootstrapSceneIndexes)
            {
                SceneManager.LoadScene(index, LoadSceneMode.Additive);
                await Task.Yield();
            }
            foreach (Scene scene in LoadedScenesCache.CachedScenes)
            {
                SceneManager.LoadScene(scene.buildIndex);
                await Task.Yield();
            }
        }
#endif
    }
}
