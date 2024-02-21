using SunsetSystems.Entities.Characters;
using InsaneSystems.RTSSelection;
using SunsetSystems.Combat;
using SunsetSystems.Game;
using SunsetSystems.Utils.Input;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Linq;
using SunsetSystems.Party;
using SunsetSystems.Spellbook;
using SunsetSystems.Entities.Characters.Interfaces;
using SunsetSystems.Entities.Characters.Actions;
using SunsetSystems.Combat.Grid;
using SunsetSystems.Entities.Interfaces;
using Sirenix.OdinInspector;

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
            if (gameplayInputHandlers.TryGetValue(GameManager.Instance.CurrentState, out IGameplayInputHandler handler))
                handler.HandlePointerPosition(context);
        }
    }
}
