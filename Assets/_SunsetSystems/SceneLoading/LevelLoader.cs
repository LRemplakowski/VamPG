using System;
using SunsetSystems.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using SunsetSystems.Utils;
using Redcode.Awaiting;
using UnityEngine.Events;
using SunsetSystems.Game;

namespace SunsetSystems.Persistence
{
    public class LevelLoader : Singleton<LevelLoader>
    {
        private SceneLoadingUIManager LoadingScreenUI => this.FindFirstComponentWithTag<SceneLoadingUIManager>(TagConstants.SCENE_LOADING_UI);

        public LevelLoadingData CachedTransitionData { get; private set; }
        private int _latestLoadedSceneIndex;

        public static Action<LevelLoadingEventData> OnBeforeLevelLoad, OnAfterLevelLoad, OnAfterSaveLoad;

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
            CachedTransitionData = data;
            await PrepareLoadingScreen();
            if (_latestLoadedSceneIndex > 1)
            {
                await UnloadGameScene();
            }
            LevelLoadingEventData loadingEventData = GetLevelLoadingEventData();
            OnBeforeLevelLoad?.Invoke(loadingEventData);
            await LoadNewScene(data);
            await IInitialized.InitializeObjectsAsync();
            OnAfterLevelLoad?.Invoke(loadingEventData);
            await new WaitForUpdate();
            //await InitializeSceneLogic(data);
            await new WaitForUpdate();
            await IInitialized.LateInitializeObjectsAsync();
            await new WaitForUpdate();
            await DisableLoadingScreen();
        }

        internal async Task LoadSavedLevel(Action preLoadingAction)
        {
            GameManager.CurrentState = GameState.GamePaused;
            await PrepareLoadingScreen();
            if (_latestLoadedSceneIndex > 1)
            {
                await UnloadGameScene();
            }
            LevelLoadingData data = new IndexLoadingData(SaveLoadManager.GetSavedSceneIndex(), "", "", preLoadingAction);
            CachedTransitionData = data;
            LevelLoadingEventData loadingEventData = GetLevelLoadingEventData();
            OnBeforeLevelLoad?.Invoke(loadingEventData);
            await LoadNewScene(data);
            await IInitialized.InitializeObjectsAsync();
            OnAfterLevelLoad?.Invoke(loadingEventData);
            await new WaitForUpdate();
            SaveLoadManager.LoadObjects();
            await IInitialized.LateInitializeObjectsAsync();
            OnAfterSaveLoad?.Invoke(loadingEventData);
            await DisableLoadingScreen();
            GameManager.CurrentState = GameState.Exploration;
        }

        private async Task PrepareLoadingScreen()
        {
            await LoadingScreenUI.DoFadeOutAsync(.5f);
            LoadingScreenUI.EnableAndResetLoadingScreen();
            await new WaitForUpdate();
            await LoadingScreenUI.DoFadeInAsync(.5f);
        }

        private async Task DisableLoadingScreen()
        {
            await LoadingScreenUI.DoFadeOutAsync(.5f);
            LoadingScreenUI.DisableLoadingScreen();
            await new WaitForUpdate();
            await LoadingScreenUI.DoFadeInAsync(.5f);
        }

        private LevelLoadingEventData GetLevelLoadingEventData()
        {
            LevelLoadingEventData data = new();
            data.AreaEntryPointTag = CachedTransitionData.targetEntryPointTag;
            data.CameraBoundingBoxTag = CachedTransitionData.cameraBoundingBoxTag;
            return data;
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
            await UnityEngine.Resources.UnloadUnusedAssets();

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

    public struct LevelLoadingEventData
    {
        public string CameraBoundingBoxTag;
        public string AreaEntryPointTag;
    }
}