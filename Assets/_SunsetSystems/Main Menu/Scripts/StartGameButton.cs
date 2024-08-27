using System.Threading.Tasks;
using SunsetSystems.Persistence;
using UnityEngine;
using SunsetSystems.Utils.Threading;
using System;

namespace SunsetSystems.MainMenu.UI
{
    public class StartGameButton : MonoBehaviour
    {
        [SerializeField]
        private GameObject backgroundSelectionObject;
        [SerializeField]
        private GameObject mainMenuParent;

        public async void StartGame()
        {
            throw new NotImplementedException();
        }

        private void DoLoadCharacterCreationPanel()
        {
            if (backgroundSelectionObject)
                backgroundSelectionObject.SetActive(true);
            mainMenuParent.SetActive(false);
        }
    }
}
