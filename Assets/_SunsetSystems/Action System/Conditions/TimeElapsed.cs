using UnityEngine;

namespace SunsetSystems.ActionSystem
{
    public class TimeElapsed : Condition
    {
        [SerializeField]
        private float _startTime, _duration;

        public TimeElapsed(float duration)
        {
            _startTime = Time.time;
            _duration = duration;
        }

        public override bool IsMet()
        {
            return Time.time >= _startTime + _duration;
        }
    }
}
