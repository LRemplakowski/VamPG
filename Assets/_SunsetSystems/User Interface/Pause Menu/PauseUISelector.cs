using Sirenix.OdinInspector;
using UnityEngine;

namespace SunsetSystems.UI
{
    public class PauseUISelector : MonoBehaviour
    {
        [SerializeField, Required]
        private PauseMenuScreenHandler _pauseMenuHandler;
        [SerializeField]
        private PauseMenuScreen _targetGuiScreen;

        public void SelectGUIScreen()
        {
            _pauseMenuHandler.OpenMenuScreen(_targetGuiScreen);
        }
    }
}
