using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShaderCrew.SeeThroughShader
{
    public abstract class ToogleByWhateverAbstract : MonoBehaviour
    {
        protected TransitionController seeThroughShaderController;

        private bool initialized = false;


        [Obsolete("Do not override Start(). Use OnStart() instead.", true)]
        void Start()
        {
            if (this.isActiveAndEnabled)
            {
                InitializeToggle();
                OnStart();
            }

        }

        // Abstract method that derived classes must implement
        protected abstract void OnStart();

        protected abstract void OnDestroyImpl();

        //protected abstract void OnDisableImpl();
        //private void OnDisable()
        //{
        //    if (seeThroughShaderController != null)
        //    {
        //        seeThroughShaderController.destroy();
        //    }
        //    OnDisableImpl();
        //}

        void OnDestroy()
        {
            if (seeThroughShaderController != null)
            {
                seeThroughShaderController.destroy();
            }

            OnDestroyImpl();
        }
        private void InitializeToggle()
        {
            if (!initialized)
            {
                seeThroughShaderController = new TransitionController(this.transform);
                initialized = true;
            }


        }

        public void activateSTSEffect()
        {
            seeThroughShaderController.notifyOnTriggerEnter(this.transform);

        }

        public void deactivateSTSEffect()
        {
            seeThroughShaderController.notifyOnTriggerExit(this.transform);

        }



    }
}