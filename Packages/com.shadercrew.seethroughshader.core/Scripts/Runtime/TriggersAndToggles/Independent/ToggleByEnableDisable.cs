using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShaderCrew.SeeThroughShader
{
    public class ToggleByEnableDisable : ToogleByWhateverAbstract
    {
        public bool invertActivationOrder = false;


        protected override void OnStart()
        {
            if (invertActivationOrder)
            {
                activateSTSEffect();
            }
        }

        void OnEnable()
        {
            if (seeThroughShaderController != null)
            {
                if (invertActivationOrder)
                {
                    activateSTSEffect();
                }
                else
                {
                    deactivateSTSEffect();
                }
            }
        }

        void OnDisable()
        {
            if (seeThroughShaderController != null)
            {
                if (invertActivationOrder)
                {
                    deactivateSTSEffect();
                }
                else
                {
                    activateSTSEffect();
                }
            }
        }

        protected override void OnDestroyImpl()
        {
        }
    }
}