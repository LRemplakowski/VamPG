namespace Utils.Scenes
{
    using System;
    using System.Collections;
    using SunsetSystems.Management;
    using Transitions.Data;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using Utils.Singleton;

    public class SceneLoader : Singleton<SceneLoader>
    {
        private LoadingScreenController loadingScreenController;

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
            SceneManager.SetActiveScene(scene);
            SceneInitializer.InitializeSingletons();
                
        }

        private IEnumerator LoadScene(int sceneIndex)
        {
            Debug.Log("Loading scene " + sceneIndex);
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneIndex);
            return HandleLoadingOperation(op);
        }

        private IEnumerator LoadScene(string sceneName)
        {
            Debug.Log("Loading scene " + sceneName);
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
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
            if (loadingScreenController == null)
                loadingScreenController = FindObjectOfType<LoadingScreenController>(true);
            switch (data.transitionType)
            {
                case Transitions.TransitionType.index:
                    int index = (int)data.get();
                    if (index != SceneManager.GetActiveScene().buildIndex)
                        StartCoroutine(LoadScene(index));
                    break;
                case Transitions.TransitionType.name:
                    string name = (string)data.get();
                    if (!name.Equals(SceneManager.GetActiveScene().name))
                        StartCoroutine(LoadScene((string)data.get()));
                    break;
                default:
                    Debug.LogException(new ArgumentException("Unhandled area transition type!"));
                    break;
            }
        }
    }
}