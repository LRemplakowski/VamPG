using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems
{
    public interface IEffect
    {
        bool ApplyEffect(IEffectTarget target);
    }

    public interface IEffectTarget
    {
        void HandleEffect(IEffect effect);
    }
}
