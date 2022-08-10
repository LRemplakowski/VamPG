using SunsetSystems.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using SunsetSystems.Utils.Threading;

namespace SunsetSystems.Bootstraper
{
    public class SunsetBootstraper : Singleton<SunsetBootstraper>
    {
        [SerializeField]
        private List<SceneAsset> bootstrapScenes = new();
        private static bool didLoadGameplayScene = false;

#if UNITY_EDITOR
        protected async override void Awake()
        {
            base.Awake();
            List<string> bootstrapScenePaths = new();
            bootstrapScenes.ForEach(sc => bootstrapScenePaths.Add(AssetDatabase.GetAssetOrScenePath(sc)));
            await Task.WhenAll(LoadScenesByPathAsync(bootstrapScenePaths));
            didLoadGameplayScene = LoadedScenesCache.CachedScenes.Count > 0;
            await Task.WhenAll(LoadScenesByPathAsync(LoadedScenesCache.CachedScenes));
            await Task.Delay(1000);
            if (didLoadGameplayScene)
            {
                PlayerInputHandler.Instance.SetPlayerInputActive(true);
                if (this.TryFindFirstGameObjectWithTag(TagConstants.MAIN_MENU_UI, out GameObject mainMenu))
                    mainMenu.SetActive(false);
                else
                    Debug.LogWarning("No Main Menu UI parent found!");
            }
            else
            {
                Debug.Log("There were no cached scenes to load!");
            }
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
                            Debug.Log("Loading scene: " + path);
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
