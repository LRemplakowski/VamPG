using System.Collections.Generic;
using System.Threading.Tasks;
using Redcode.Awaiting;
using SunsetSystems.Core.SceneLoading.UI;
using SunsetSystems.Utils;
using UltEvents;
using UnityEngine;

namespace SunsetSystems.Core.SceneLoading
{
    public class LevelLoader : Singleton<LevelLoader>
    {
        [SerializeField]
        private SceneLoadingUIManager loadingScreenUI;
        [SerializeField]
        private float loadingCrossfadeTime = 1f;
        [SerializeField]
        private UltEvent OnLoadingStart, OnLoadingEnd;

        public async Task LoadNewScene(SceneLoadingData data)
        {
            await loadingScreenUI.DoFadeOutAsync(loadingCrossfadeTime / 2f);
            OnLoadingStart?.InvokeSafe();
            loadingScreenUI.EnableAndResetLoadingScreen();
            await new WaitForUpdate();
            await loadingScreenUI.DoFadeInAsync(loadingCrossfadeTime / 2f);
            await DoSceneLoading(data);
            await loadingScreenUI.DoFadeOutAsync(loadingCrossfadeTime / 2f);
            loadingScreenUI.DisableLoadingScreen();
            OnLoadingEnd?.InvokeSafe();
            await new WaitForUpdate();
            await loadingScreenUI.DoFadeInAsync(loadingCrossfadeTime / 2f);

        }

        private async Task DoSceneLoading(SceneLoadingData data)
        {
            var asyncOp = UnityEngine.AddressableAssets.Addressables.LoadSceneAsync(data.AddressableScenePaths[0], UnityEngine.SceneManagement.LoadSceneMode.Single);
            while (asyncOp.IsDone == false)
            {
                loadingScreenUI.UpadteLoadingBar(asyncOp.PercentComplete);
                await Task.Yield();
            }

            List<Task> loadingOps = new();
            for (int i = 1; i < data.AddressableScenePaths.Count; i++)
            {
                loadingOps.Add(UnityEngine.AddressableAssets.Addressables.LoadSceneAsync(data.AddressableScenePaths[i], UnityEngine.SceneManagement.LoadSceneMode.Additive).Task);
            }
            await Task.WhenAll(loadingOps);
        }
    }
}