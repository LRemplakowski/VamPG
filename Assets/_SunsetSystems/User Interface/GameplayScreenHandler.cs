using Sirenix.OdinInspector;
using SunsetSystems.Entities;
using SunsetSystems.Game;
using SunsetSystems.Inventory.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;

namespace SunsetSystems.UI
{
    public class GameplayScreenHandler : SerializedMonoBehaviour
    {
        [SerializeField]
        private NameplateManager _nameplateManager;

        public void OnPointerPositionAction(InputAction.CallbackContext context)
        {
            _nameplateManager.OnPointerPosition(context);
        }

        public void OnHighlightInteractablesAction(InputAction.CallbackContext context)
        {
            _nameplateManager.OnHighlightInteractables(context);
        }
    }
}
