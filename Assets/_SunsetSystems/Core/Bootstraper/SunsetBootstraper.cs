using SunsetSystems.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using Redcode.Awaiting;

namespace SunsetSystems.Bootstraper
{
    public class SunsetBootstraper : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField]
        private List<SceneAsset> bootstrapScenes = new();
        private static bool didLoadGameplayScene = false;

        public static bool EnableBootstrap { get; set; }

        protected async void Awake()
        {
            List<string> bootstrapScenePaths = new();
            bootstrapScenes.ForEach(sc => bootstrapScenePaths.Add(AssetDatabase.GetAssetOrScenePath(sc)));
            await Task.WhenAll(LoadScenesByPathAsync(bootstrapScenePaths));
            if (!EnableBootstrap)
                return;
            didLoadGameplayScene = LoadedScenesCache.CachedScenes.Count > 0;
            await Task.WhenAll(LoadScenesByPathAsync(LoadedScenesCache.CachedScenes));
            await Task.Delay(1000);
            if (didLoadGameplayScene)
            {
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
                tasks.Add(Task.Run(async () =>
                {
                    await new WaitForUpdate();
                    if (!SceneManager.GetSceneByPath(path).isLoaded)
                    {
                        Debug.Log("Loading scene: " + path);
                        await EditorSceneManager.LoadSceneAsyncInPlayMode(path, parameters);
                    }
                }));
            }
            return tasks;
        }
#endif
    }
}
