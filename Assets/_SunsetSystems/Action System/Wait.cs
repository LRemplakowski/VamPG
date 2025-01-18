using System;
using SunsetSystems.Abilities;
using UnityEngine;

namespace SunsetSystems.ActionSystem
{
    public class Wait : EntityAction, ICloneable
    {
        [SerializeField]
        private float _waitDuration;

        public Wait(float duration, IActionPerformer owner) : base(owner)
        {
            _waitDuration = duration;
        }

        public override void Begin()
        {
            conditions.Clear();
            conditions.Add(new TimeElapsed(_waitDuration));
        }

        public object Clone()
        {
            return new Wait(_waitDuration, Owner);
        }
    }
}
