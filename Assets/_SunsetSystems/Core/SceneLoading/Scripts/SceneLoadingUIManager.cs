using System.Threading.Tasks;
using UnityEngine;

namespace SunsetSystems.Core.SceneLoading.UI
{
    public class SceneLoadingUIManager : MonoBehaviour
    {
        [SerializeField]
        private LoadingScreenController loadingScreen;
        [SerializeField]
        private FadeScreenAnimator fadeScreen;

        public static SceneLoadingUIManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else if (Instance != this)
                Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
        private void Start()
        {
            if (loadingScreen == null)
                loadingScreen = GetComponentInChildren<LoadingScreenController>();
            if (fadeScreen == null)
                fadeScreen = GetComponentInChildren<FadeScreenAnimator>();
        }

        public async Task DoFadeOutAsync(float fadeOutTime) => await fadeScreen.FadeOut(fadeOutTime);

        public async Task DoFadeInAsync(float fadeInTime) => await fadeScreen.FadeIn(fadeInTime);

        public void UpadteLoadingBar(float value) => loadingScreen.SetLoadingProgress(value);

        public void EnableAndResetLoadingScreen()
        {
            loadingScreen.gameObject.SetActive(true);
        }

        public void DisableLoadingScreen()
        {
            loadingScreen.gameObject.SetActive(false);
        }
    }
}
