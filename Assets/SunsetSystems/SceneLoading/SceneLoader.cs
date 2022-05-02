using System;
using SunsetSystems.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

namespace SunsetSystems.Loading
{
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
            _previousScene = scene;
            SceneManager.SetActiveScene(scene);
        }

        internal async Task LoadGameScene(SceneLoadingData data)
        {
            await _fadeScreenAnimator.FadeOut(.5f);
            Debug.Log("enabling loading screen");
            _loadingScreenController.gameObject.SetActive(true);
            await _fadeScreenAnimator.FadeIn(.5f);
            await LoadNewScene(data);
            await InitializeSceneLogic(data);
            _loadingScreenController.EnableContinue();
        }

        internal async Task LoadSavedScene(Action preLoadingAction)
        {
            await _fadeScreenAnimator.FadeOut(.5f);
            Debug.Log("enabling loading screen");
            _loadingScreenController.gameObject.SetActive(true);
            await Task.Yield();
            await _fadeScreenAnimator.FadeIn(.5f);
            SceneLoadingData data = new IndexLoadingData(SaveLoadManager.GetSavedSceneIndex(), "", "", preLoadingAction);
            await LoadNewScene(data);
            await SaveLoadManager.LoadObjects();
            await InitializeSceneLogic(data);
            _loadingScreenController.EnableContinue();
        }

        internal async Task LoadSavedScene()
        {
            await LoadSavedScene(null);
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
            Debug.Log("Performing pre-loading action");
            if (data.preLoadingActions != null)
                foreach (Action action in data.preLoadingActions)
                {
                    action.Invoke();
                    await Task.Yield();
                }
            if (_previousScene.buildIndex != 0)
                await UnloadPreviousScene();
            await DoSceneLoading();
        }

        private async Task InitializeSceneLogic(SceneLoadingData data)
        {
            AbstractSceneLogic sceneLogic = FindObjectOfType<AbstractSceneLogic>();
            Debug.Log("Scene logic found? " + (sceneLogic != null));
            if (sceneLogic)
                await sceneLogic.StartSceneAsync(data);
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