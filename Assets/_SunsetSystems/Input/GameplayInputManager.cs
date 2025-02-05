using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using SunsetSystems.Game;
using SunsetSystems.Utils.Input;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace SunsetSystems.Input
{
    public class GameplayInputManager : SerializedMonoBehaviour
    {
        [Title("References")]
        [SerializeField]
        private Dictionary<GameState, IGameplayInputHandler> gameplayInputHandlers = new();

        private Vector2 mousePosition;

        private void OnEnable()
        {
            SunsetInputHandler.OnPrimaryAction += OnPrimaryAction;
            SunsetInputHandler.OnSecondaryAction += OnSecondaryAction;
            SunsetInputHandler.OnPointerPosition += OnPointerPosition;
        }

        private void OnDisable()
        {
            SunsetInputHandler.OnPrimaryAction -= OnPrimaryAction;
            SunsetInputHandler.OnSecondaryAction -= OnSecondaryAction;
            SunsetInputHandler.OnPointerPosition -= OnPointerPosition;
        }

        private static bool DoesAnyUIHitBlockRaycasts(List<RaycastResult> hits)
        {
            return hits.Any((hit) => { var cg = hit.gameObject.GetComponentInParent<CanvasGroup>(); return cg != null && cg.blocksRaycasts; });
        }

        private void OnPrimaryAction(InputAction.CallbackContext context)
        {
            if ((InputHelper.IsRaycastHittingUIObject(mousePosition, out List<RaycastResult> hits) && DoesAnyUIHitBlockRaycasts(hits)) is false)
            {
                if (gameplayInputHandlers.TryGetValue(GameManager.Instance.CurrentState, out IGameplayInputHandler handler))
                    handler.HandlePrimaryAction(context);
            }

        }

        private void OnSecondaryAction(InputAction.CallbackContext context)
        {
            if ((InputHelper.IsRaycastHittingUIObject(mousePosition, out List<RaycastResult> hits) && DoesAnyUIHitBlockRaycasts(hits)) is false)
            {
                if (gameplayInputHandlers.TryGetValue(GameManager.Instance.CurrentState, out IGameplayInputHandler handler))
                    handler.HandleSecondaryAction(context);
            }
        }

        private void OnPointerPosition(InputAction.CallbackContext context)
        {
            if (context.performed)
                mousePosition = context.ReadValue<Vector2>();
            if (gameplayInputHandlers.TryGetValue(GameManager.Instance.CurrentState, out IGameplayInputHandler handler))
                handler.HandlePointerPosition(context);
        }
    }
}
