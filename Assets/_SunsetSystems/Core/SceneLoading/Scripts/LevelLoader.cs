using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Redcode.Awaiting;
using SunsetSystems.Core.SceneLoading.UI;
using SunsetSystems.Persistence;
using SunsetSystems.Utils;
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
        private Camera loadingCamera;

        public SceneLoadingData CurrentLevelAsset { get; private set; }

        public static event Action OnLevelLoadStart, OnLevelLoadEnd, OnBeforePersistentDataLoad;
        public static event Action OnAfterScreenFadeOut, OnBeforeScreenFadeIn;

        public async Task LoadNewScene(SceneLoadingData data)
        {
            await loadingScreenUI.DoFadeOutAsync(loadingCrossfadeTime / 2f);
            await new WaitForUpdate();
            loadingCamera.gameObject.SetActive(true);
            SaveLoadManager.UpdateRuntimeDataCache();
            OnLevelLoadStart?.Invoke();
            loadingScreenUI.EnableAndResetLoadingScreen();
            await new WaitForSeconds(.5f);
            await loadingScreenUI.DoFadeInAsync(loadingCrossfadeTime / 2f);
            await DoSceneLoading(data);
            CurrentLevelAsset = data;
            await new WaitForUpdate();
            OnBeforePersistentDataLoad?.Invoke();
            SaveLoadManager.InjectRuntimeDataIntoSaveables();
            await new WaitForSeconds(0.1f);
            OnLevelLoadEnd?.Invoke();
            await loadingScreenUI.DoFadeOutAsync(loadingCrossfadeTime / 2f);
            loadingCamera.gameObject.SetActive(false);
            loadingScreenUI.DisableLoadingScreen();
            await new WaitForSeconds(.1f);
            await loadingScreenUI.DoFadeInAsync(loadingCrossfadeTime / 2f);
        }

        public async Task LoadSavedGame(string saveID)
        {
            await loadingScreenUI.DoFadeOutAsync(loadingCrossfadeTime / 2f);
            await new WaitForUpdate();
            loadingCamera.gameObject.SetActive(true);
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