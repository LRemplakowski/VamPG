using SunsetSystems.Persistence.UI;
using SunsetSystems.Utils;
using System.Threading.Tasks;
using UnityEngine;

namespace SunsetSystems.Persistence
{
    [RequireComponent(typeof(Tagger))]
    public class SceneLoadingUIManager : MonoBehaviour
    {
        [SerializeField]
        private LoadingScreenController loadingScreen;
        [SerializeField]
        private FadeScreenAnimator fadeScreen;
        // Start is called before the first frame update
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
