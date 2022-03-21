namespace Utils.Scenes
{
    using SunsetSystems.GameData;
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
        private LoadingScreenController loadingScreenController;
        [SerializeField]
        private FadeScreenAnimator fadeScreenAnimator;
        private Scene previousScene;

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
            previousScene = SceneManager.GetSceneAt(0);
        }

        private void Start()
        {
            if (loadingScreenController == null)
                loadingScreenController = FindObjectOfType<LoadingScreenController>(true);
            if (fadeScreenAnimator == null)
                fadeScreenAnimator = FindObjectOfType<FadeScreenAnimator>(true);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (previousScene != SceneManager.GetSceneAt(0))
                SceneManager.UnloadSceneAsync(previousScene);
            previousScene = scene;
            SceneManager.SetActiveScene(scene);
        }

        internal async Task LoadGameScene(SceneLoadingData data)
        {
            await LoadScene(data);
            InitializeSceneLogic();
        }

        internal async Task LoadSavedScene(SceneLoadingData data)
        {
            await LoadScene(data);
            SaveLoadManager.OnRuntimeDataLoaded += InitializeSceneLogic;
        }

        private async Task LoadScene(SceneLoadingData data)
        {
            CachedTransitionData = data;
            EnableLoadingScreen();
            await DoSceneLoading();
        }

        private void EnableLoadingScreen()
        {
            loadingScreenController.gameObject.SetActive(true);
        }
        
        private void InitializeSceneLogic()
        {
            AbstractSceneLogic sceneLogic = FindObjectOfType<AbstractSceneLogic>();
            Debug.Log("Scene logic found? " + sceneLogic != null);
            if (sceneLogic)
                sceneLogic.StartScene();
            SaveLoadManager.OnRuntimeDataLoaded -= InitializeSceneLogic;
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
                loadingScreenController.SetLoadingProgress(progress);
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

        public async Task DoSceneLoading()
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