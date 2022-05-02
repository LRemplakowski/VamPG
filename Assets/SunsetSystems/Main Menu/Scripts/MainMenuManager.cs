using SunsetSystems.Constants;
using UnityEngine;

namespace SunsetSystems.MainMenu
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject debugUi;

        // Start is called before the first frame update
        void Start()
        {
            if (debugUi)
                debugUi.SetActive(GameConstants.DEBUG_MODE);
        }
    }
}
