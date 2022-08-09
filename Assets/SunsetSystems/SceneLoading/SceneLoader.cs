using System;
using SunsetSystems.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using SunsetSystems.Utils.Threading;
using SunsetSystems.Utils;
using SunsetSystems.Constants;

namespace SunsetSystems.Loading
{
    public class SceneLoader : Singleton<SceneLoader>
    {
        private Scene _previousScene;
        private SceneLoadingUIManager LoadingScreenUI => this.FindFirstComponentWithTag<SceneLoadingUIManager>(TagConstants.SCENE_LOADING_UI);

        public SceneLoadingData CachedTransitionData { get; private set; }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        protected override void Awake()
        {
            base.Awake();
            _previousScene = new Scene();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            _previousScene = scene;
            SceneManager.SetActiveScene(scene);
        }

        internal async Task LoadGameScene(SceneLoadingData data)
        {
            await LoadingScreenUI.DoFadeOutAsync(.5f);
            LoadingScreenUI.EnableAndResetLoadingScreen();
            await UnityAwaiters.NextFrame();
            await LoadingScreenUI.DoFadeInAsync(.5f);
            // Don't know why, but _previousScene can return true for IsValid() even for invalid scenes, like the one created in Awake.
            // Checking for -1 buildIndex works around this issue.
            if (_previousScene.IsValid() && _previousScene.buildIndex != -1 && _previousScene.buildIndex != GameConstants.GAME_SCENE_INDEX && _previousScene.buildIndex != GameConstants.UI_SCENE_INDEX)
            {
                await UnloadScene(_previousScene.buildIndex);
            }
            await LoadNewScene(data);
            await InitializeSceneLogic(data);
            await LoadingScreenUI.DoFadeOutAsync(.5f);
            LoadingScreenUI.DisableLoadingScreen();
            await UnityAwaiters.NextFrame();
            await LoadingScreenUI.DoFadeInAsync(.5f);
        }

        internal async Task LoadSavedScene(Action preLoadingAction)
        {
            await LoadingScreenUI.DoFadeOutAsync(.5f);
            LoadingScreenUI.EnableAndResetLoadingScreen();
            await UnityAwaiters.NextFrame();
            await LoadingScreenUI.DoFadeInAsync(.5f);
            // Don't know why, but _previousScene can return true for IsValid() even for invalid scenes, like the one created in Awake.
            // Checking for -1 buildIndex works around this issue.
            if (_previousScene.IsValid() && _previousScene.buildIndex != -1 && _previousScene.buildIndex != GameConstants.GAME_SCENE_INDEX && _previousScene.buildIndex != GameConstants.UI_SCENE_INDEX)
            {
                await UnloadScene(_previousScene.buildIndex);
            }
            SceneLoadingData data = new IndexLoadingData(SaveLoadManager.GetSavedSceneIndex(), "", "", preLoadingAction);
            await LoadNewScene(data);
            await SaveLoadManager.LoadObjects();
            await InitializeSceneLogic(data);
            await LoadingScreenUI.DoFadeOutAsync(.5f);
            LoadingScreenUI.DisableLoadingScreen();
            await UnityAwaiters.NextFrame();
            await LoadingScreenUI.DoFadeInAsync(.5f);
        }

        internal async Task LoadSavedScene()
        {
            await LoadSavedScene(null);
        }

        private Task UnloadScene(int sceneIndex)
        {
            return Task.Run(() =>
            {
                Dispatcher.Instance.Invoke(async () =>
                {
                    AsyncOperation op = SceneManager.UnloadSceneAsync(sceneIndex);
                    await HandleUnloadingOperation(op);
                });
            });
        }

        private async Task HandleUnloadingOperation(AsyncOperation unloading)
        {
            while (!unloading.isDone)
            {
                await Task.Yield();
            }
        }

        private async Task LoadNewScene(SceneLoadingData data)
        {
            CachedTransitionData = data;
            Debug.Log("Performing pre-loading action");
            if (data.preLoadingActions != null)
                foreach (Action action in data.preLoadingActions)
                {
                    action?.Invoke();
                    await Task.Yield();
                }
            await DoSceneLoading();
        }

        private async Task InitializeSceneLogic(SceneLoadingData data)
        {
            await Task.Run(() =>
            {
                Dispatcher.Instance.Invoke(async () =>
                {
                    AbstractSceneLogic sceneLogic = FindObjectOfType<AbstractSceneLogic>();
                    Debug.Log("Scene logic found? " + (sceneLogic != null).ToString());
                    if (sceneLogic)
                        await sceneLogic.StartSceneAsync(data);
                });
            });
        }

        private Task AsyncLoadSceneByIndex(int sceneIndex)
        {
            Debug.Log("Loading scene " + sceneIndex);
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
            return HandleLoadingOperation(op);
        }

        private Task AsyncLoadSceneByName(string sceneName)
        {
            Debug.Log("Loading scene " + sceneName);
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            return HandleLoadingOperation(op);
        }

        private async Task HandleLoadingOperation(AsyncOperation loadingOp)
        {
            while (!loadingOp.isDone)
            {
                float progress = Mathf.Clamp01(loadingOp.progress / 0.9f);
                LoadingScreenUI.UpadteLoadingBar(progress);
                Debug.Log(progress);
                await Task.Yield();
            }
        }

        private async Task DoLoadingByIndex()
        {
            int index = (int)CachedTransitionData.Get();
            await AsyncLoadSceneByIndex(index);
        }

        private async Task DoLoadingByName()
        {
            string name = CachedTransitionData.Get() as string;
            await AsyncLoadSceneByName(name);
        }

        private async Task DoSceneLoading()
        {
            Debug.Log("Do start scene loading");
            switch (CachedTransitionData.transitionType)
            {
                case TransitionType.indexTransition:
                    await DoLoadingByIndex();
                    break;
                case TransitionType.nameTransition:
                    await DoLoadingByName();
                    break;
                default:
                    throw new ArgumentException("Invalid transition type!");
            }
        }
    }
}