namespace Utils.Scenes
{
    using Transitions.Data;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using Utils.Singleton;

    public class SceneLoader : Singleton<SceneLoader>
    {
        public void LoadScene(int sceneIndex)
        {
            Debug.Log("Loading scene " + sceneIndex);
            if (SceneManager.GetActiveScene().buildIndex != sceneIndex)
                SceneManager.LoadScene(sceneIndex);
        }

        public void LoadScene(string sceneName)
        {
            if (!SceneManager.GetActiveScene().name.Equals(sceneName))
                SceneManager.LoadScene(sceneName);
        }
    }
}