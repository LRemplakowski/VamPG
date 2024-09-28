using Sirenix.OdinInspector;
using SunsetSystems.Tooltips;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SunsetSystems.UI
{
    public class GameplayScreenHandler : SerializedMonoBehaviour
    {
        [SerializeField]
        private NameplateManager _nameplateManager;
        [SerializeField]
        private GameObject _helpMenu;

        public void OnPointerPositionAction(InputAction.CallbackContext context)
        {

        }

        public void OnHighlightInteractablesAction(InputAction.CallbackContext context)
        {
            _nameplateManager.OnHighlightInteractables(context);
        }

        public void OnHelpAction(InputAction.CallbackContext context)
        {
            if (context.started)
                _helpMenu.SetActive(true);
            else if (context.canceled)
                _helpMenu.SetActive(false);
        }
    }
}
