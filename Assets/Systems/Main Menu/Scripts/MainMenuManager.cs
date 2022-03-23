using System.Collections;
using System.Collections.Generic;
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
                debugUi.SetActive(Consts.DEBUG_MODE);
        }
    }
}
