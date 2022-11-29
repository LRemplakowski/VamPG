using CleverCrow.Fluid.UniqueIds;
using SunsetSystems.Constants;
using SunsetSystems.Game;
using SunsetSystems.Loading;
using SunsetSystems.Utils;
using System.EnterpriseServices;
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
        [SerializeField]
        private LayerMask _groundRaycastMask;

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

        private void Start()
        {
            if (!_cameraTransform)
                _cameraTransform = GetComponentInChildren<Camera>().transform;
            if (!_rotationTarget)
                _rotationTarget = transform;
            if (_target)
                transform.position = _target.position;
            _moveTarget = transform.position;
            RealignCamera();
        }

        private void OnValidate()
        {
            if (!_cameraTransform)
                _cameraTransform = GetComponentInChildren<Camera>().transform;
            RealignCamera();
        }

        [ContextMenu("Realign camera")]
        private void RealignCamera()
        {
            _cameraTransform.localPosition = _offset;
            _cameraTransform.LookAt(_rotationTarget);
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
            if (GameManager.CurrentState == GameState.Conversation)
                _moveDirection = Vector3.zero;
        }

        public void OnRotate(InputAction.CallbackContext context)
        {
            if (!(context.performed || context.canceled))
                return;
            _rotationDirection = -context.ReadValue<Vector2>().x;
            if (GameManager.CurrentState == GameState.Conversation)
                _rotationDirection = 0;
        }

        public void ForceToPosition(Vector3 position)
        {
            transform.position = position;
            _moveTarget = transform.position;
            Debug.Log("Forcing camera to position " + position);
        }

        public void ForceRotation(Vector3 eulerAngles)
        {
            transform.localEulerAngles = eulerAngles;
        }

        private void FixedUpdate()
        {
            _internalMoveTargetSpeed = _cameraMoveSpeed + 1f;
            if (_moveDirection.z != 0)
                _moveTarget += _internalMoveTargetSpeed * _moveDirection.z * Time.fixedDeltaTime * transform.forward;
            if (_moveDirection.x != 0)
                _moveTarget += _internalMoveTargetSpeed * _moveDirection.x * Time.fixedDeltaTime * transform.right;
            Ray ray = new(transform.position + (Vector3.up * _offset.y), -Vector3.up);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, _groundRaycastMask, QueryTriggerInteraction.Collide))
            {
                _moveTarget = new(_moveTarget.x, hitInfo.point.y, _moveTarget.z);
            }
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
            ES3.Save(Unique.Id + BOUNDING_BOX, _currentBoundingBox);
            ES3.Save(Unique.Id + POSITION, transform.position);
        }

        public void LoadRuntimeData()
        {
            _currentBoundingBox = ES3.Load<BoundingBox>(Unique.Id + BOUNDING_BOX);
            ForceToPosition(ES3.Load<Vector3>(Unique.Id + POSITION));
        }
    }
}
