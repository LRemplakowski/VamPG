using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Utils
{
    public class TimeManager : MonoBehaviour
    {
        [ShowInInspector, ReadOnly]
        private float _currentTimeScale = 1f;
        [ShowInInspector, PropertyRange(0f, 20f), OnValueChanged("OnTargetTimeScaleChanged")]
        private float _targetTimeScale = 1f;

        // Start is called before the first frame update
        void Start()
        {
            _currentTimeScale = Time.timeScale;
        }

        // Update is called once per frame
        void Update()
        {
            _currentTimeScale = Time.timeScale;
        }

        private void OnTargetTimeScaleChanged()
        {
            Time.timeScale = _targetTimeScale;
        }
    }
}
