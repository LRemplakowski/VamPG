using UnityEngine;
using UnityEngine.InputSystem;

namespace SunsetSystems.Input.CameraControl
{
    public class CameraControlScript : ExposableMonobehaviour
    {
        private Transform target;
        [SerializeField]
        private Transform cameraTransform;
        [SerializeField]
        private Transform rotationTarget;
        [SerializeField]
        private BoundingBox currentBoundingBox;
        public Vector3 offset;

        //Movement variables
        private float internalMoveTargetSpeed;
        [SerializeField]
        private float cameraMoveSpeed = 4.0f, cameraRotationSpeed = 15f;
        private Vector3 moveTarget;
        private Vector3 moveDirection;
        private float rotationDirection;

        private void Start()
        {
            currentBoundingBox = FindObjectOfType<BoundingBox>();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            if (!(context.performed || context.canceled))
                return;
            Vector2 value = context.ReadValue<Vector2>();
            if (value.x == 0 && value.y == 0)
            {
                moveDirection = Vector3.zero;
            }
            moveDirection = new Vector3(value.x, 0, value.y);
        }

        public void OnRotate(InputAction.CallbackContext context)
        {
            if (!(context.performed || context.canceled))
                return;
            rotationDirection = -context.ReadValue<Vector2>().x;
        }

        public void ForceToPosition(Vector3 position)
        {
            transform.position = position;
            moveTarget = transform.position;
            Debug.Log("Forcing camera to position " + position);
        }

        public void Initialize()
        {
            if (cameraTransform == null)
                cameraTransform = GetComponentInChildren<Camera>().transform;
            target = GameManager.GetMainCharacter().transform;
            if (target)
            {
                transform.position = target.position;
                moveTarget = transform.position;
                cameraTransform.localPosition = offset;
                cameraTransform.LookAt(rotationTarget);
            }
        }

        private void FixedUpdate()
        {
            internalMoveTargetSpeed = cameraMoveSpeed + 1f;
            if (moveDirection.z != 0)
                moveTarget += transform.forward * moveDirection.z * Time.fixedDeltaTime * internalMoveTargetSpeed;
            if (moveDirection.x != 0)
                moveTarget += transform.right * moveDirection.x * Time.fixedDeltaTime * internalMoveTargetSpeed;
            if (currentBoundingBox)
                moveTarget = currentBoundingBox.IsPositionWithinBounds(moveTarget) ? moveTarget : currentBoundingBox.ClampPositionToBounds(moveTarget);
        }

        private void LateUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, moveTarget, Time.deltaTime * cameraMoveSpeed);
            transform.Rotate(Vector3.up * Time.deltaTime * cameraRotationSpeed * rotationDirection);
        }
    }
}
