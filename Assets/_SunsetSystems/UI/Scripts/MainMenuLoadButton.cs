using SunsetSystems.Persistence;
using UnityEngine;
using UnityEngine.UI;

namespace SunsetSystems.UI
{
    public class MainMenuLoadButton : MonoBehaviour
    {
        [SerializeField]
        private Button _button;

        private void OnEnable()
        {
            _button.interactable = SaveLoadManager.HasExistingSaves();
        }
    }
}
