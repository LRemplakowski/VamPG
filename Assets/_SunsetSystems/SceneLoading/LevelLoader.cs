using System;
using SunsetSystems.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using SunsetSystems.Utils.Threading;
using SunsetSystems.Utils;
using SunsetSystems.Constants;
using Redcode.Awaiting;

namespace SunsetSystems.Loading
{
    public class LevelLoader : Singleton<LevelLoader>
    {
        private SceneLoadingUIManager LoadingScreenUI => this.FindFirstComponentWithTag<SceneLoadingUIManager>(TagConstants.SCENE_LOADING_UI);

        public LevelLoadingData CachedTransitionData { get; private set; }
        private int _latestLoadedSceneIndex;

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
            _latestLoadedSceneIndex = -1;
        }

        private void Start()
        {
#if !UNITY_EDITOR
            SceneManager.LoadScene("UI", LoadSceneMode.Additive);
#endif
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            _latestLoadedSceneIndex = scene.buildIndex;
            SceneManager.SetActiveScene(scene);
        }

        public async Task LoadGameLevel(LevelLoadingData data)
        {
            await LoadingScreenUI.DoFadeOutAsync(.5f);
            LoadingScreenUI.EnableAndResetLoadingScreen();
            await new WaitForUpdate();
            await LoadingScreenUI.DoFadeInAsync(.5f);
            // Don't know why, but _previousScene can return true for IsValid() even for invalid scenes, like the one created in Awake.
            // Checking for -1 buildIndex works around this issue.
            if (_latestLoadedSceneIndex > 1)
            {
                _ = UnloadGameScene();
            }
            await LoadNewScene(data);
            await InitializeSceneLogic(data);
            await LoadingScreenUI.DoFadeOutAsync(.5f);
            LoadingScreenUI.DisableLoadingScreen();
            await new WaitForUpdate();
            await LoadingScreenUI.DoFadeInAsync(.5f);
        }

        internal async Task LoadSavedLevel(Action preLoadingAction)
        {
            await LoadingScreenUI.DoFadeOutAsync(.5f);
            LoadingScreenUI.EnableAndResetLoadingScreen();
            await new WaitForUpdate();
            await LoadingScreenUI.DoFadeInAsync(.5f);
            // Don't know why, but _previousScene can return true for IsValid() even for invalid scenes, like the one created in Awake.
            // Checking for -1 buildIndex works around this issue.
            if (_latestLoadedSceneIndex > 1)
            {
                _ = UnloadGameScene();
            }
            LevelLoadingData data = new IndexLoadingData(SaveLoadManager.GetSavedSceneIndex(), "", "", preLoadingAction);
            await LoadNewScene(data);
            SaveLoadManager.LoadObjects();
            await LoadingScreenUI.DoFadeOutAsync(.5f);
            LoadingScreenUI.DisableLoadingScreen();
            await new WaitForUpdate();
            await LoadingScreenUI.DoFadeInAsync(.5f);
        }

        internal async Task LoadSavedLevel()
        {
            await LoadSavedLevel(null);
        }

        private async Task LoadNewScene(LevelLoadingData data)
        {
            CachedTransitionData = data;
            if (data.preLoadingActions != null)
                foreach (Action action in data.preLoadingActions)
                {
                    action?.Invoke();
                }
            await DoSceneLoading();
        }

        private async Task InitializeSceneLogic(LevelLoadingData data)
        {
            AbstractSceneLogic sceneLogic = this.FindFirstComponentWithTag<AbstractSceneLogic>(TagConstants.SCENE_LOGIC);
            if (sceneLogic)
                await sceneLogic.StartSceneAsync(data);
        }

        private async Task DoLoadingByIndex()
        {
            int index = (int)CachedTransitionData.Get();
            AsyncOperation loadingOp = SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);
            while (!loadingOp.isDone)
            {
                float progress = Mathf.Clamp01(loadingOp.progress / 0.9f);
                LoadingScreenUI.UpadteLoadingBar(progress);
                await Task.Yield();
            }
        }

        public async Task UnloadGameScene()
        {
            await SceneManager.UnloadSceneAsync(_latestLoadedSceneIndex);
        }

        private async Task DoLoadingByName()
        {
            string name = CachedTransitionData.Get() as string;
            AsyncOperation loadingOp = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
            while (!loadingOp.isDone)
            {
                float progress = Mathf.Clamp01(loadingOp.progress / 0.9f);
                LoadingScreenUI.UpadteLoadingBar(progress);
                await Task.Yield();
            }
        }

        private async Task DoSceneLoading()
        {
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