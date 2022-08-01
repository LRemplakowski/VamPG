using System.Threading.Tasks;
using SunsetSystems.Loading;
using UnityEngine;
using SunsetSystems.Utils.Threading;

namespace SunsetSystems.MainMenu.UI
{
    public class StartGameButton : MonoBehaviour
    {
        [SerializeField]
        private SceneLoadingUIManager fadeUI;
        [SerializeField]
        private GameObject backgroundSelectionObject;
        [SerializeField]
        private GameObject mainMenuParent;

        private void Start()
        {
            if (fadeUI == null)
                fadeUI = this.FindFirstComponentWithTag<SceneLoadingUIManager>(TagConstants.SCENE_LOADING_UI);
        }

        public async void StartGame()
        {
            await Task.Run(() =>
            {
                Dispatcher.Instance.Invoke(async () =>
                {
                    await fadeUI.DoFadeOutAsync(.5f);
                    DoLoadCharacterCreationPanel();
                    await fadeUI.DoFadeInAsync(.5f);
                });
            });
        }

        private void DoLoadCharacterCreationPanel()
        {
            if (backgroundSelectionObject)
                backgroundSelectionObject.SetActive(true);
            mainMenuParent.SetActive(false);

        }
    }
}
