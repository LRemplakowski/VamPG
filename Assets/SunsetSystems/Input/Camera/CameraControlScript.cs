using CleverCrow.Fluid.UniqueIds;
using SunsetSystems.Constants;
using SunsetSystems.Loading;
using SunsetSystems.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SunsetSystems.Input.CameraControl
{
    [RequireComponent(typeof(UniqueId))]
    [RequireComponent(typeof(Tagger))]
    public class CameraControlScript : ExposableMonobehaviour, ISaveRuntimeData
    {
        private Transform _target;
        [SerializeField]
        private Transform _cameraTransform;
        [SerializeField]
        private Transform _rotationTarget;
        [SerializeField]
        private BoundingBox _currentBoundingBox;
        public BoundingBox CurrentBoundingBox { set => _currentBoundingBox = value; }
        [SerializeField]
        private Vector3 _offset;

        //Movement variables
        private float _internalMoveTargetSpeed;
        [SerializeField]
        private float _cameraMoveSpeed = 4.0f, _cameraRotationSpeed = 15f;
        private Vector3 _moveTarget;
        private Vector3 _moveDirection;
        private float _rotationDirection;

        //Save/Load variables
        private UniqueId Unique => GetComponent<UniqueId>();
        private const string BOUNDING_BOX = "_boundingBox";
        private const string POSITION = "_position";

        private void Awake()
        {
            if (!_cameraTransform)
                _cameraTransform = GetComponentInChildren<Camera>().transform;
            if (!_rotationTarget && this.TryFindFirstGameObjectWithTag(TagConstants.CAMERA_FOCUS, out GameObject result))
                _rotationTarget = result.transform;
            if (_target)
            {
                transform.position = _target.position;
                _moveTarget = transform.position;
                _cameraTransform.localPosition = _offset;
                _cameraTransform.LookAt(_rotationTarget);
            }
        }

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

        private void FixedUpdate()
        {
            _internalMoveTargetSpeed = _cameraMoveSpeed + 1f;
            if (_moveDirection.z != 0)
                _moveTarget += _internalMoveTargetSpeed * _moveDirection.z * Time.fixedDeltaTime * transform.forward;
            if (_moveDirection.x != 0)
                _moveTarget += _internalMoveTargetSpeed * _moveDirection.x * Time.fixedDeltaTime * transform.right;
            if (_currentBoundingBox)
                _moveTarget = _currentBoundingBox.IsPositionWithinBounds(_moveTarget) ? _moveTarget : _currentBoundingBox.ClampPositionToBounds(_moveTarget);
        }

        private void LateUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, _moveTarget, Time.deltaTime * _cameraMoveSpeed);
            transform.Rotate(_cameraRotationSpeed * _rotationDirection * Time.deltaTime * Vector3.up);
        }

        public void SaveRuntimeData()
        {
            ES3.Save<BoundingBox>(Unique.Id + BOUNDING_BOX, _currentBoundingBox);
            ES3.Save<Vector3>(Unique.Id + POSITION, transform.position);
        }

        public void LoadRuntimeData()
        {
            _currentBoundingBox = ES3.Load<BoundingBox>(Unique.Id + BOUNDING_BOX);
            ForceToPosition(ES3.Load<Vector3>(Unique.Id + POSITION));
        }
    }
}
