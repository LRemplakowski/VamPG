using CleverCrow.Fluid.UniqueIds;
using SunsetSystems.Game;
using SunsetSystems.Persistence;
using SunsetSystems.Utils;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

namespace SunsetSystems.Input.CameraControl
{
    [RequireComponent(typeof(UniqueId))]
    [RequireComponent(typeof(Tagger))]
    public class CameraControlScript : MonoBehaviour, ISaveable
    {
        [SerializeField]
        public float lookOffset;
        [SerializeField]
        public float cameraAngle;
        [SerializeField]
        public float defaultZoom;
        [SerializeField]
        public float zoomMax;
        [SerializeField]
        public float zoomMin;
        [SerializeField]
        public float rotationSpeed;
        
        [SerializeField]
        private static LayerMask _groundRaycastMask;
        [SerializeField]
        private Transform _rotationTarget;
        [SerializeField]
        private Camera _mainCamera;
        [SerializeField]
        private Vector3 _cameraPositionTarget;
        [SerializeField]
        private BoundingBox _currentBoundingBox;
        public BoundingBox CurrentBoundingBox { set => _currentBoundingBox = value; }

        //Save/Load variables
        private UniqueId Unique => GetComponent<UniqueId>();
        public string DataKey => Unique.Id;


        private void Awake()
        {
            ISaveable.RegisterSaveable(this);
        }

        private void OnEnable()
        {
            LevelLoader.OnAfterLevelLoad += OnAfterLevelLoad;
        }

        private void OnDisable()
        {
            LevelLoader.OnAfterLevelLoad -= OnAfterLevelLoad;
        }

        private void OnDestroy()
        {
            ISaveable.UnregisterSaveable(this);
        }

        private void OnAfterLevelLoad(LevelLoadingEventData data)
        {
            BoundingBox boundingBox = this.FindFirstComponentWithTag<BoundingBox>(data.CameraBoundingBoxTag);
            if (boundingBox is not null)
                CurrentBoundingBox = boundingBox;
            Waypoint entryPoint = this.FindFirstComponentWithTag<Waypoint>(data.AreaEntryPointTag);
            if (entryPoint is not null)
                ForceToPosition(entryPoint.transform.position);
        }

        private const float internalMoveTargetSpeed = 8;
        private const float internalMoveSpeed = 4;
        private Vector3 _moveTarget;
        private Vector3 _moveDirection;
        private float _rotationDirection;
        private Vector2 _zoomDirection;
        private static Vector2 mousePos;
        private static Vector3 mousePosVector;

        private float _currentZoomAmount;
        public float currentZoom
        {   
            get => _currentZoomAmount;
            private set
            {
                _currentZoomAmount = value;
                UpdateCameraTarget();
            }
        }
        private float _internalZoomSpeed = 4;

        private void UpdateCameraTarget()
        {
            _cameraPositionTarget = (Vector3.up * lookOffset) + 
                (Quaternion.AngleAxis(cameraAngle, Vector3.right) * Vector3.back) * _currentZoomAmount;
        }

        void Start()
        {
            if (!_mainCamera){
                _mainCamera = GetComponentInChildren<Camera>();
            }
            if(_moveTarget != Vector3.zero){
                _mainCamera.transform.position = _cameraPositionTarget;
                _moveTarget = transform.position;
            }
            _mainCamera.transform.rotation = Quaternion.AngleAxis(cameraAngle, Vector3.right);
            currentZoom = defaultZoom;
            _mainCamera.transform.position = _cameraPositionTarget;
            _rotationTarget = transform;
            RealignCamera();
        }


        private void RealignCamera()
        {
            _mainCamera.transform.localPosition = Vector3.zero;
            _mainCamera.transform.LookAt(_rotationTarget);
        }

        public void ForceToPosition(Vector3 position)
        {
            _mainCamera.transform.position = position;
            _moveTarget = transform.position;
            Debug.Log("Forcing camera to position " + position);
        }

        public void ForceRotation(Vector3 eulerAngles)
        {
            _mainCamera.transform.localEulerAngles = eulerAngles;
        }
        
        public void OnMove(InputAction.CallbackContext context)
        {
            if (!(context.canceled || context.performed)){
                return;
            }
            Vector2 value = context.ReadValue<Vector2>();
            _moveDirection = new Vector3(value.x, 0, value.y);
            if (GameManager.CurrentState == GameState.Conversation){
                _moveDirection = Vector3.zero;
            }
            else if (GameManager.CurrentState == GameState.Combat){
                return;
            }
        }

        public void OnZoom(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Performed)
            {
                return;
            }
            if (!(context.performed || context.canceled)){
                return;
            }
            if (GameManager.CurrentState == GameState.Conversation){
                _zoomDirection = Vector2.zero;
            }
            else if (GameManager.CurrentState == GameState.Combat){
                _zoomDirection = context.ReadValue<Vector2>();
                currentZoom = Mathf.Clamp(_currentZoomAmount - _zoomDirection.y, zoomMin, zoomMax);
            }
            
        }

        public void OnRotate(InputAction.CallbackContext context)
        {
            if (!(context.performed || context.canceled))
                return;
            _rotationDirection = -context.ReadValue<Vector2>().x;
            if (GameManager.CurrentState == GameState.Conversation)
                _rotationDirection = 0;
            else if (GameManager.CurrentState == GameState.Combat){
                return;
            }
        }

        private void FixedUpdate()
        {
            _moveTarget += (transform.forward * _moveDirection.z + transform.right *
            _moveDirection.x) * Time.fixedDeltaTime * internalMoveTargetSpeed;
            Ray ray = new(transform.position + Vector3.up, -Vector3.up);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, _groundRaycastMask, QueryTriggerInteraction.Collide)){
                _moveTarget = new(_moveTarget.x, hitInfo.point.y, _moveTarget.z);
            }
            if (_currentBoundingBox){
                _moveTarget = _currentBoundingBox.IsPositionWithinBounds(_moveTarget) ? _moveTarget : _currentBoundingBox.ClampPositionToBounds(_moveTarget);
            }
        }

        public static Vector3 GetPosition(){
            mousePos = new Vector2(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue());
            mousePosVector = new Vector3(mousePos.x, 0, mousePos.y);
            Ray ray = Camera.main.ScreenPointToRay(mousePosVector);
            Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, _groundRaycastMask);
            return raycastHit.point;
        }

        private void LateUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, _moveTarget, Time.deltaTime * internalMoveSpeed);
            _mainCamera.transform.localPosition = Vector3.Lerp(_mainCamera.transform.localPosition, _cameraPositionTarget, Time.deltaTime * _internalZoomSpeed);
            transform.Rotate(rotationSpeed * _rotationDirection * Time.deltaTime * Vector3.up);
        }

        public object GetSaveData()
        {
            CameraSaveData saveData = new();
            saveData.CurrentBoundingBoxTag = _currentBoundingBox?.GetComponent<Tagger>().tag ?? "";
            saveData.RigPosition = transform.position;
            saveData.CameraMoveTarget = _moveTarget;
            saveData.CameraRotationTarget = _rotationTarget.localEulerAngles;
            return saveData;
        }

        public void InjectSaveData(object data)
        {
            CameraSaveData saveData = data as CameraSaveData;
            _currentBoundingBox = this.FindFirstComponentWithTag<BoundingBox>(saveData.CurrentBoundingBoxTag);
            ForceToPosition(saveData.RigPosition);
            _moveTarget = saveData.CameraMoveTarget;
            _rotationTarget.localEulerAngles = saveData.CameraRotationTarget;
        }
    }

    public class CameraSaveData : SaveData
    {
        public string CurrentBoundingBoxTag;
        public Vector3 RigPosition;
        public Vector3 CameraMoveTarget, CameraRotationTarget;
    }
}
