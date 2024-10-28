using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SunsetSystems.WorldMap;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace SunsetSystems.Input
{
    public class WorldMapInputHandler : SerializedMonoBehaviour, IGameplayInputHandler
    {
        [SerializeField]
        private LayerMask _interactionRaycastLayers;
        [SerializeField]
        private WorldMapUI _worldMapUIManager;

        private IWorldMapToken _lastHitToken;
        private Vector2 _pointerPosition;

        public void HandlePointerPosition(InputAction.CallbackContext context)
        {
            _pointerPosition = context.ReadValue<Vector2>();
            Ray ray = Camera.main.ScreenPointToRay(_pointerPosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 200, _interactionRaycastLayers, QueryTriggerInteraction.Collide))
            {
                hit.collider.TryGetComponent(out _lastHitToken);
            }
        }

        public void HandlePrimaryAction(InputAction.CallbackContext context)
        {

        }

        public void HandleSecondaryAction(InputAction.CallbackContext context)
        {
            if (context.started && _lastHitToken == null)
                _worldMapUIManager.LockTokenDescription(false, null);
        }
    }
}
