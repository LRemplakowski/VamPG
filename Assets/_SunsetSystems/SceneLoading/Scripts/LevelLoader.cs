using System.Collections.Generic;
using System.Threading.Tasks;
using SunsetSystems.UI;
using SunsetSystems.Utils;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SunsetSystems.SceneLoading
{
    public class LevelLoader : Singleton<LevelLoader>
    {
        [SerializeField]
        private SceneLoadingUIManager loadingScreenUI;
        [SerializeField]
        private float loadingCrossfadeTime = 1f;

        public async Task LoadNewScene(SceneLoadingData data)
        {
            await loadingScreenUI.DoFadeOutAsync(loadingCrossfadeTime / 2);
            loadingScreenUI.EnableAndResetLoadingScreen();
            await DoSceneLoading(data);
        }

        private async Task DoSceneLoading(SceneLoadingData data)
        {
            var asyncOp = Addressables.LoadSceneAsync(data.AddressableScenePaths[0], UnityEngine.SceneManagement.LoadSceneMode.Single);
            while (asyncOp.IsDone == false)
            {
                loadingScreenUI.UpadteLoadingBar(asyncOp.PercentComplete);
                await Task.Yield();
            }

            List<Task> loadingOps = new();
            for (int i = 1; i < data.AddressableScenePaths.Count; i++)
            {
                loadingOps.Add(Addressables.LoadSceneAsync(data.AddressableScenePaths[i], UnityEngine.SceneManagement.LoadSceneMode.Additive).Task);
            }
            await Task.WhenAll(loadingOps);
        }
    }
}