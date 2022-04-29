using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.UI.Pause
{
    public class PauseUISelector : MonoBehaviour
    {
        [SerializeField]
        private GameObject _targetGuiScreen;
        private static GameObject _currentGuiScreen;

        public void SelectGUIScreen()
        {
            if (_currentGuiScreen)
                _currentGuiScreen.SetActive(false);
            if (_targetGuiScreen)
                _targetGuiScreen.SetActive(true);
            _currentGuiScreen = _targetGuiScreen;
        }
    }
}
