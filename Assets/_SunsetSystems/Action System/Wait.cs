using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SunsetSystems.ActionSystem
{
    public class Wait : EntityAction, ICloneable
    {
        [SerializeField]
        private IWaitTimeProvider _waitDurationProvider;

        public Wait(float duration, IActionPerformer owner) : base(owner)
        {
            _waitDurationProvider = new ConstantWaitTime(duration);
        }

        public Wait(float minDuration, float maxDuration, IActionPerformer owner) : base(owner)
        {
            _waitDurationProvider = new RandomWaitTime(minDuration, maxDuration);
        }

        private Wait(IWaitTimeProvider waitTimeProvider, IActionPerformer owner) : base(owner)
        {
            _waitDurationProvider = waitTimeProvider;
        }

        private Wait(Wait from) : this(from._waitDurationProvider, from.Owner)
        {

        }

        public override void Begin()
        {
            conditions.Clear();
            conditions.Add(new TimeElapsed(_waitDurationProvider.GetWaitTime()));
        }

        public object Clone()
        {
            return new Wait(this);
        }

#if UNITY_EDITOR
        [Button(DirtyOnClick = true)]
        private void SetWaitTime(float waitTime)
        {
            _waitDurationProvider = new ConstantWaitTime(waitTime);
        }

        [Button(DirtyOnClick = true)]
        private void SetWaitTime(float minTime, float maxTime)
        {
            _waitDurationProvider = new RandomWaitTime(minTime, maxTime);
        }
#endif

        private interface IWaitTimeProvider
        {
            float GetWaitTime();
        }

        [Serializable]
        private class ConstantWaitTime : IWaitTimeProvider
        {
            [SerializeField]
            private float _waitDuration;

            public ConstantWaitTime(float waitDuration) => _waitDuration = waitDuration;

            public float GetWaitTime()
            {
                return _waitDuration;
            }
        }

        [Serializable]
        private class RandomWaitTime : IWaitTimeProvider
        {
            [SerializeField]
            private float _minDuration, _maxDuration;

            public RandomWaitTime(float minDuration, float maxDuration)
            {
                _minDuration = minDuration;
                _maxDuration = maxDuration;
            }

            public float GetWaitTime()
            {
                return UnityEngine.Random.Range(_minDuration, _maxDuration);
            }
        }
    }
}
