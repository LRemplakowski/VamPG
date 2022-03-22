namespace SunsetSystems.Scenes
{
    using SunsetSystems.Data;
    using System;
    using System.Collections;
    using Transitions.Data;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using Transitions.Manager;
    using SunsetSystems.SaveLoad;
    using System.Threading.Tasks;

    public class SceneLoader : MonoBehaviour
    {
        [SerializeField]
        private LoadingScreenController _loadingScreenController;
        [SerializeField]
        private FadeScreenAnimator _fadeScreenAnimator;
        private Scene _previousScene;

        public SceneLoadingData CachedTransitionData { get; private set; }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void Awake()
        {
            _previousScene = SceneManager.GetSceneAt(0);
        }

        private void Start()
        {
            if (_loadingScreenController == null)
                _loadingScreenController = FindObjectOfType<LoadingScreenController>(true);
            if (_fadeScreenAnimator == null)
                _fadeScreenAnimator = FindObjectOfType<FadeScreenAnimator>(true);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (_previousScene != SceneManager.GetSceneAt(0))
                SceneManager.UnloadSceneAsync(_previousScene);
            _previousScene = scene;
            SceneManager.SetActiveScene(scene);
        }

        internal async Task LoadGameScene(SceneLoadingData data)
        {
            await _fadeScreenAnimator.FadeOut(.5f);
            _loadingScreenController.gameObject.SetActive(true);
            await _fadeScreenAnimator.FadeIn(.5f);
            await LoadNewScene(data);
            InitializeSceneLogic();
        }

        internal async Task LoadSavedScene()
        {
            await _fadeScreenAnimator.FadeOut(.5f);
            _loadingScreenController.gameObject.SetActive(true);
            await _fadeScreenAnimator.FadeIn(.5f);
            SceneLoadingData data = new IndexLoadingData(SaveLoadManager.GetSavedSceneIndex(), "");
            await LoadNewScene(data);
            await SaveLoadManager.LoadObjects();
            InitializeSceneLogic();
        }

        private Task UnloadPreviousScene()
        {
            AsyncOperation op = SceneManager.UnloadSceneAsync(_previousScene.buildIndex);
            return HandleUnloadingOperation(op);
        }

        private async Task HandleUnloadingOperation(AsyncOperation unloading)
        {
            while (!unloading.isDone)
            {
                float progress = Mathf.Clamp01(unloading.progress / 0.9f);
                _loadingScreenController.SetUnloadingProgress(progress);
                await Task.Yield();
            }
        }

        private async Task LoadNewScene(SceneLoadingData data)
        {
            CachedTransitionData = data;
            if (data.preLoadingAction != null)
                data.preLoadingAction.Invoke();
            if (_previousScene.buildIndex != 0)
                await UnloadPreviousScene();
            await DoSceneLoading();
        }
        
        private void InitializeSceneLogic()
        {
            AbstractSceneLogic sceneLogic = FindObjectOfType<AbstractSceneLogic>();
            Debug.Log("Scene logic found? " + (sceneLogic != null));
            if (sceneLogic)
                sceneLogic.StartScene();
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
                _loadingScreenController.SetLoadingProgress(progress);
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
                case Transitions.TransitionType.index:
                    await DoLoadingByIndex();
                    break;
                case Transitions.TransitionType.name:
                    await DoLoadingByName();
                    break;
                default:
                    throw new ArgumentException("Invalid transition type!");
            }
        }
    }
}