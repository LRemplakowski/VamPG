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

#if !UNITY_EDITOR
        private void Start()
        {
            SceneManager.LoadScene("UI", LoadSceneMode.Additive);
        }
#endif

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
                SaveLoadManager.UpdateRuntimeDataCache();
                await UnloadGameScene();
            }
            LevelLoadingEventData loadingEventData = GetLevelLoadingEventData();
            OnBeforeLevelLoad?.Invoke(loadingEventData);
            await LoadNewScene(data);
            await IInitialized.InitializeObjectsAsync();
            await new WaitForUpdate();
            await InitializeSceneLogic(data);
            await new WaitForUpdate();
            SaveLoadManager.InjectRuntimeDataIntoSaveables();
            await IInitialized.LateInitializeObjectsAsync();
            OnAfterLevelLoad?.Invoke(loadingEventData);
            await new WaitForUpdate();
            await DisableLoadingScreen();
            GameManager.CurrentState = GameState.Exploration;
            //Debug.Break();
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
            await new WaitForUpdate();
            SaveLoadManager.LoadSavedDataIntoRuntime();
            SaveLoadManager.InjectRuntimeDataIntoSaveables();
            OnAfterSaveLoad?.Invoke(loadingEventData);
            await IInitialized.LateInitializeObjectsAsync();
            OnAfterLevelLoad?.Invoke(loadingEventData);
            await new WaitForUpdate();
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
            while (loadingOp.isDone is false)
            {
                float progress = Mathf.Clamp01(loadingOp.progress / 0.9f);
                LoadingScreenUI.UpadteLoadingBar(progress);
                Debug.Log($"Loading progress: {progress}");
                await new WaitForUpdate();
            }
        }

        public async Task UnloadGameScene()
        {
            AsyncOperation op = SceneManager.UnloadSceneAsync(_latestLoadedSceneIndex);
            await new WaitUntil(() => op.isDone);
            op = UnityEngine.Resources.UnloadUnusedAssets();
            await new WaitUntil(() => op.isDone);
        }

        private async Task DoLoadingByName()
        {
            string name = CachedTransitionData.Get() as string;
            if (string.IsNullOrWhiteSpace(name))
                return;
            AsyncOperation loadingOp = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
            while (loadingOp.isDone is not true)
            {
                float progress = Mathf.Clamp01(loadingOp.progress / 0.9f);
                LoadingScreenUI.UpadteLoadingBar(progress);
                await new WaitForUpdate();
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