using SunsetSystems.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InsaneSystems.RTSSelection.UI
{
    [RequireComponent(typeof(Tagger))]
    /// <summary> This class describes selection UI rectangle, which will visually show multiselection area. </summary>
    public class SelectionRect : MonoBehaviour
    {
        [SerializeField] RectTransform selfTransform;

        Vector2 startPoint;
        private Vector2 mousePosition;

        bool wasActivated;

        void Awake()
        {
            if (!selfTransform)
                selfTransform = GetComponent<RectTransform>();

            selfTransform.pivot = new Vector2(0.5f, 0.5f);

            if (!wasActivated)
                DisableRect();
        }

        private void OnEnable()
        {
            PlayerInputHandler.OnMousePositionEvent += OnMousePosition;
        }

        private void OnDisable()
        {
            PlayerInputHandler.OnMousePositionEvent -= OnMousePosition;
        }

        public void EnableRect(Vector3 atScreenPoint)
        {
            wasActivated = true;

            selfTransform.sizeDelta = Vector2.zero;
            startPoint = atScreenPoint;

            gameObject.SetActive(true);
        }

        void Update()
        {
            var actualMousePosition = mousePosition;
            var midPoint = Vector2.Lerp(startPoint, actualMousePosition, 0.5f);

            selfTransform.transform.position = midPoint;

            var rectXSize = Mathf.Abs(actualMousePosition.x - startPoint.x);
            var rectYSize = Mathf.Abs(actualMousePosition.y - startPoint.y);

            selfTransform.sizeDelta = new Vector2(rectXSize, rectYSize);
        }

        public void OnMousePosition(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Performed)
                return;
            mousePosition = context.ReadValue<Vector2>();
        }

        public void DisableRect() => gameObject.SetActive(false);
    }
}