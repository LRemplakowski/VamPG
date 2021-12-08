namespace Utils.Scenes
{
    using Glitchers;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Transitions.Data;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using Utils.Singleton;

    public class SceneLoader : Singleton<SceneLoader>
    {
        [SerializeField]
        private LoadingScreenController loadingScreenController;

        public static TransitionData CachedTransitionData { get; private set; }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (loadingScreenController == null)
                loadingScreenController = FindObjectOfType<LoadingScreenController>(true);
            Debug.Log("LoadSceneMode: " + loadSceneMode + "; AdditiveModeIntvalue: " + LoadSceneMode.Additive);
            if (loadSceneMode == LoadSceneMode.Additive)
            {
                Debug.LogError("Load scene mode additive enter");
                Scene previousScene = SceneManager.GetActiveScene();
                List<IMoveBetweenScenes> moveables = FindInterfaces.Find<IMoveBetweenScenes>();
                Debug.LogError(moveables.Count + " objects to move");
                foreach (IMoveBetweenScenes moveable in moveables)
                {
                    Debug.LogError("moving object " + moveable.GetGameObject() + " to scene " + scene.name);
                    SceneManager.MoveGameObjectToScene(moveable.GetGameObject(), scene);
                }
                SceneManager.SetActiveScene(scene);
                SceneManager.UnloadSceneAsync(previousScene);
            }
            SceneInitializer.InitializeSingletons();
            SceneInitializer.InitializePlayableCharacters();
        }

        private IEnumerator LoadScene(int sceneIndex)
        {
            Debug.Log("Loading scene " + sceneIndex);
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneIndex, CachedTransitionData.loadSceneMode);
            return HandleLoadingOperation(op);
        }

        private IEnumerator LoadScene(string sceneName)
        {
            Debug.Log("Loading scene " + sceneName);
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, CachedTransitionData.loadSceneMode);
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
                    Debug.LogException(new ArgumentException("Unhandled area transition type!"));
                    break;
            }
        }
    }
}