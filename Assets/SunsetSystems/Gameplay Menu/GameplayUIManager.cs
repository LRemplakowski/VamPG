using SunsetSystems.Utils;
using System;
using UnityEngine;

namespace SunsetSystems.GameplayMenu
{
    [RequireComponent(typeof(Tagger))]
    public class GameplayUIManager : MonoBehaviour
    {
        [SerializeField]
        private Canvas _gameplayCanvas;

        // Start is called before the first frame update
        private void Start()
        {
            if (!_gameplayCanvas)
                throw new NullReferenceException("Gampeplay Canvas reference is missing in " + this);
            else
                _gameplayCanvas.gameObject.SetActive(false);
        }

        public void SetGameplayUiActive(bool isActive)
        {
            _gameplayCanvas.gameObject.SetActive(isActive);
        }
    }
}
