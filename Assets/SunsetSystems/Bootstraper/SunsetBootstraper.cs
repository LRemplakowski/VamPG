using SunsetSystems.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.Threading;

namespace SunsetSystems.Bootstraper
{
    public class SunsetBootstraper : Singleton<SunsetBootstraper>
    {
        [SerializeField]
        private List<SceneAsset> bootstrapScenes = new();

#if UNITY_EDITOR
        protected async override void Awake()
        {
            base.Awake();
            List<string> bootstrapScenePaths = new();
            bootstrapScenes.ForEach(sc => bootstrapScenePaths.Add(AssetDatabase.GetAssetOrScenePath(sc)));
            await Task.WhenAll(LoadScenesByPathAsync(bootstrapScenePaths));
            await Task.WhenAll(LoadScenesByPathAsync(LoadedScenesCache.CachedScenes));
        }

        private List<Task> LoadScenesByPathAsync(List<string> paths)
        {
            List<Task> tasks = new();
            LoadSceneParameters parameters = new();
            parameters.loadSceneMode = LoadSceneMode.Additive;
            parameters.localPhysicsMode = LocalPhysicsMode.Physics3D;
            foreach (string path in paths)
            {
                tasks.Add(Task.Run(() =>
                {
                    Dispatcher.Instance.Invoke(async () =>
                    {
                        if (!SceneManager.GetSceneByPath(path).isLoaded)
                        {
                            AsyncOperation op = EditorSceneManager.LoadSceneAsyncInPlayMode(path, parameters);
                            while (!op.isDone)
                            {
                                await Task.Yield();
                            }
                        }
                    });
                }));
            }
            return tasks;
        }
#endif
    }
}
