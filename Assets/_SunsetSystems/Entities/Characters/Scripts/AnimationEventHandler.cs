using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UltEvents;
using UnityEngine;

namespace SunsetSystems.Animation
{
    public class AnimationEventHandler : SerializedMonoBehaviour
    {
        public UltEvent<string> OnAnimationEvent = new();

        public void FireAnimationEvent(string eventType)
        {
            OnAnimationEvent?.InvokeSafe(eventType);
        }
    }
}
