using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;
using UnityEngine;

namespace ShaderCrew.SeeThroughShader
{

    // This STS Trigger just sets up the manual player-based trigger and exposes a public function to activate and deactive
    // Only works with PlayerBased settings!
    public class ManualTriggerByParent : MonoBehaviour
    {
        TransitionController seeThroughShaderController;

        void Start()
        {
            if (this.isActiveAndEnabled)
            {
                InitializeTrigger();
            }
        }


        private void InitializeTrigger()
        {
            Transform parentTransform = parentTransform = transform;
            
            if (parentTransform != null)
            {
                seeThroughShaderController = new TransitionController(parentTransform);
            }
        }

        public void ActivateTrigger(Collider other)
        {
            if (this.isActiveAndEnabled && other.gameObject.GetComponent<SeeThroughShaderPlayer>() != null)
            {
                seeThroughShaderController.notifyOnTriggerEnter(this.transform, other.transform);
            }
        }

        public void DeactivateTrigger(Collider other)
        {
            if (this.isActiveAndEnabled && other.gameObject.GetComponent<SeeThroughShaderPlayer>() != null)
            {
                seeThroughShaderController.notifyOnTriggerExit(this.transform, other.transform);

            }            
        }
    }
}