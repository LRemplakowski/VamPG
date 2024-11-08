using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SunsetSystems.Utils
{
    public class RotationRandomizer : MonoBehaviour
    {
        [SerializeField]
        private bool _editorOnly = false;
        [SerializeField, Required]
        private Transform _rotationTransform;
        [MinMaxSlider(-360, 360, ShowFields = true)]
        [SerializeField]
        private Vector2 _xRotation, _yRotation, _zRotation;
        [SerializeField]
        private bool _destroyAfterStart = true;

        private void Awake()
        {
            if (_rotationTransform == null)
                _rotationTransform = transform;
        }

        private void Start()
        {
            if (_editorOnly)
            {
                Destroy(this);
                return;
            }
            RandomizeRotation();
            if (_destroyAfterStart)
                Destroy(this);
        }

        [Button]
        private void RandomizeRotation()
        {
            Vector3 eulerAngles = Vector3.zero;
            eulerAngles.x = Random.Range(_xRotation.x, _xRotation.y);
            eulerAngles.y = Random.Range(_yRotation.x, _yRotation.y);
            eulerAngles.z = Random.Range(_zRotation.x, _zRotation.y);
            _rotationTransform.rotation = Quaternion.Euler(eulerAngles);
        }
    }
}
