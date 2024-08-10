using UnityEngine;

namespace SunsetSystems.Entities.Characters.Actions
{
    public class TimeElapsed : Condition
    {
        [SerializeField]
        private float _startTime, _duration;

        public TimeElapsed(float startTime, float duration)
        {
            _startTime = startTime;
            _duration = duration;
        }

        public override bool IsMet()
        {
            return Time.time >= _startTime + _duration;
        }
    }
}
