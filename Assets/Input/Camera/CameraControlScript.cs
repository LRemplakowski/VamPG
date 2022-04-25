using UnityEngine;
using UnityEngine.InputSystem;

namespace SunsetSystems.Input.CameraControl
{
    public class CameraControlScript : ExposableMonobehaviour
    {
        private Transform _target;
        [SerializeField]
        private Transform _cameraTransform;
        [SerializeField]
        private Transform _rotationTarget;
        [SerializeField]
        private BoundingBox _currentBoundingBox;
        [SerializeField]
        private Vector3 _offset;

        //Movement variables
        private float _internalMoveTargetSpeed;
        [SerializeField]
        private float _cameraMoveSpeed = 4.0f, _cameraRotationSpeed = 15f;
        private Vector3 _moveTarget;
        private Vector3 _moveDirection;
        private float _rotationDirection;

        public void OnMove(InputAction.CallbackContext context)
        {
            if (!(context.performed || context.canceled))
                return;
            Vector2 value = context.ReadValue<Vector2>();
            if (value.x == 0 && value.y == 0)
            {
                _moveDirection = Vector3.zero;
            }
            _moveDirection = new Vector3(value.x, 0, value.y);
        }

        public void OnRotate(InputAction.CallbackContext context)
        {
            if (!(context.performed || context.canceled))
                return;
            _rotationDirection = -context.ReadValue<Vector2>().x;
        }

        public void ForceToPosition(Vector3 position)
        {
            transform.position = position;
            _moveTarget = transform.position;
            Debug.Log("Forcing camera to position " + position);
        }

        public void Initialize()
        {
            if (_cameraTransform == null)
                _cameraTransform = GetComponentInChildren<Camera>().transform;
            _target = GameManager.GetMainCharacter().transform;
            if (_target)
            {
                transform.position = _target.position;
                _moveTarget = transform.position;
                _cameraTransform.localPosition = _offset;
                _cameraTransform.LookAt(_rotationTarget);
            }
        }

        private void FixedUpdate()
        {
            _internalMoveTargetSpeed = _cameraMoveSpeed + 1f;
            if (_moveDirection.z != 0)
                _moveTarget += transform.forward * _moveDirection.z * Time.fixedDeltaTime * _internalMoveTargetSpeed;
            if (_moveDirection.x != 0)
                _moveTarget += transform.right * _moveDirection.x * Time.fixedDeltaTime * _internalMoveTargetSpeed;
            if (_currentBoundingBox)
                _moveTarget = _currentBoundingBox.IsPositionWithinBounds(_moveTarget) ? _moveTarget : _currentBoundingBox.ClampPositionToBounds(_moveTarget);
        }

        private void LateUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, _moveTarget, Time.deltaTime * _cameraMoveSpeed);
            transform.Rotate(Vector3.up * Time.deltaTime * _cameraRotationSpeed * _rotationDirection);
        }
    }
}
