
using SunsetSystems.Spellbook;
using UnityEngine;

namespace SunsetSystems.Entities.Characters.Actions
{
    public class Wait : EntityAction
    {
        [SerializeField]
        private float _waitDuration;

        public Wait(float duration, IActionPerformer owner) : base(owner)
        {
            _waitDuration = duration;
        }

        public override void Begin()
        {
            conditions.Add(new TimeElapsed(Time.time, _waitDuration));
        }
    }
}
