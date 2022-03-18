namespace Utils.Scenes
{
    using SunsetSystems.GameData;
    using System;
    using System.Collections;
    using Transitions.Data;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class SceneLoader : MonoBehaviour
    {
        [SerializeField]
        private LoadingScreenController loadingScreenController;
        private Scene previousScene;

        public TransitionData CachedTransitionData { get; private set; }

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

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (loadingScreenController == null)
                loadingScreenController = FindObjectOfType<LoadingScreenController>(true);
            if (previousScene != SceneManager.GetSceneAt(0))
                SceneManager.UnloadSceneAsync(previousScene);
            previousScene = scene;
            SceneManager.SetActiveScene(scene);
            AbstractSceneLogic sceneLogic = FindObjectOfType<AbstractSceneLogic>();
            if (sceneLogic)
                sceneLogic.StartScene();
        }

        private IEnumerator LoadScene(int sceneIndex)
        {
            Debug.Log("Loading scene " + sceneIndex);
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
            return HandleLoadingOperation(op);
        }

        private IEnumerator LoadScene(string sceneName)
        {
            Debug.Log("Loading scene " + sceneName);
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            return HandleLoadingOperation(op);
        }

        private IEnumerator HandleLoadingOperation(AsyncOperation loadingOp)
        {
            while (!loadingOp.isDone)
            {
                float progress = Mathf.Clamp01(loadingOp.progress / 0.9f);
                loadingScreenController.SetLoadingProgress(progress);
                Debug.Log(progress);

                yield return null;
            }
        }

        public void LoadScene(TransitionData data)
        {
            CachedTransitionData = data;
            if (loadingScreenController == null)
                loadingScreenController = FindObjectOfType<LoadingScreenController>(true);
            switch (data.transitionType)
            {
                case Transitions.TransitionType.index:
                    int index = (int)data.get();
                    StartCoroutine(LoadScene(index));
                    break;
                case Transitions.TransitionType.name:
                    string name = (string)data.get();
                    StartCoroutine(LoadScene(name));
                    break;
                default:
                    throw new ArgumentException("Unhandled area transition type!");
            }
        }
    }
}