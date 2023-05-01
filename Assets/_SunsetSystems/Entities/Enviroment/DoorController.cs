using UnityEngine;

namespace SunsetSystems.Entities.Enviroment
{
    public class DoorController : MonoBehaviour
    {
        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private bool _reverseAnimation = false;

        private bool _doorState = false;
        public bool Open
        {
            get => _doorState;
            set
            {
                _doorState = value;
                _animator?.SetBool(DOOR_STATE, value);
            }
        }

        private const string DOOR_STATE = "Open";
        private const string ANIMATION_REVERSE = "Reversed";

        private void Awake()
        {
            _animator ??= GetComponentInChildren<Animator>();
            _animator?.SetBool(ANIMATION_REVERSE, _reverseAnimation);
        }

        public void TriggerStateChange()
        {
            Open = !Open;
        }
    }
}
