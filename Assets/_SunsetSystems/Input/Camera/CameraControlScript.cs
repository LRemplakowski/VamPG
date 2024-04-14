using CleverCrow.Fluid.UniqueIds;
using SunsetSystems.Game;
using SunsetSystems.Persistence;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

namespace SunsetSystems.Input.CameraControl
{
    [RequireComponent(typeof(DataKeyConstants))]
    public class CameraControlScript : MonoBehaviour, ISaveable
    {
        [SerializeField]
        public float defaultZoom;
        [SerializeField]
        public float zoomMax;
        [SerializeField]
        public float zoomMin;
        [SerializeField]
        private CinemachineVirtualCamera mainVirtualCam;
        
        [SerializeField]
        private static LayerMask _groundRaycastMask;
        [SerializeField]
        private BoundingBox _currentBoundingBox;
        public BoundingBox CurrentBoundingBox { set => _currentBoundingBox = value; }

        //Save/Load variables
       
        public string DataKey => DataKeyConstants.CAMERA_CONTROL_SCRIPT_DATA_KEY;


        private void Awake()
        {
            ISaveable.RegisterSaveable(this);
        }

        private void OnDestroy()
        {
            ISaveable.UnregisterSaveable(this);
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
            }
        }
        private float _internalZoomSpeed = 4;

        private bool _movedToSavedPosition = false;

        private void Start()
        {
            _moveTarget = transform.position;
            currentZoom = defaultZoom;
        }

        public void MoveToLevelStartPosition()
        {
            if (_movedToSavedPosition is false)
                ForceToPosition(WaypointManager.Instance.GetSceneEntryWaypoint().transform);
        }

        public void ForceToPosition(Vector3 position) => _moveTarget = position;

        public void ForceToPosition(Transform positionSource) => ForceToPosition(positionSource.position);

        public void ForceRotation(Vector3 eulerAngles)
        {
            Debug.Log("Forcing camera to specified rotation");
        }
        
        public void OnMove(InputAction.CallbackContext context)
        {
            if (!(context.canceled || context.performed)){
                return;
            }
            Vector2 value = context.ReadValue<Vector2>();
            _moveDirection = new Vector3(value.x, 0, value.y);
            if (GameManager.Instance.CurrentState == GameState.Conversation){
                _moveDirection = Vector3.zero;
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
            if (GameManager.Instance.CurrentState == GameState.Conversation){
                _zoomDirection = Vector2.zero;
            }
            else if (GameManager.Instance.CurrentState == GameState.Combat){
                _zoomDirection = context.ReadValue<Vector2>();
                currentZoom = Mathf.Clamp(_currentZoomAmount - _zoomDirection.y, zoomMin, zoomMax);
            }
            
        }

        private void FixedUpdate()
        {
            _moveTarget += Quaternion.AngleAxis(mainVirtualCam.transform.localRotation.eulerAngles.y, Vector3.up) * _moveDirection * internalMoveTargetSpeed * Time.fixedDeltaTime;
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
        }

        public object GetSaveData()
        {
            CameraSaveData saveData = new();
            saveData.CurrentBoundingBox = _currentBoundingBox;
            saveData.RigPosition = _moveTarget;
            return saveData;
        }

        public void InjectSaveData(object data)
        {
            if (data is not CameraSaveData saveData)
                return;
            _currentBoundingBox = saveData.CurrentBoundingBox;
            ForceToPosition(saveData.RigPosition);
            _movedToSavedPosition = true;
        }
    }

    public class CameraSaveData : SaveData
    {
        public BoundingBox CurrentBoundingBox;
        public Vector3 RigPosition;
        public Vector3 CameraRotationTarget;
    }
}
