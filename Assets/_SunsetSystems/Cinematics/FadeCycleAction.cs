using System.Collections;
using System.Collections.Generic;
using UltEvents;
using UnityEngine;

namespace SunsetSystems.Cinematics
{
    public class FadeCycleAction : MonoBehaviour
    {
        public UltEvent OnAfterFadeOut = new();

        public void Execute()
        {
            FadeScreenManager.Instance.CycleFade(WhenScreenFaded);
        }

        private void WhenScreenFaded()
        {
            OnAfterFadeOut?.InvokeSafe();
        }
    }
}
