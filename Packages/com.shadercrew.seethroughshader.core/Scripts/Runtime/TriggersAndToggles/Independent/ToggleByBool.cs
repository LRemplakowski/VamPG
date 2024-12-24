using ShaderCrew.SeeThroughShader;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShaderCrew.SeeThroughShader
{
    public class ToggleByBool : ToogleByWhateverAbstract
    {

        private bool _isActivated = false;

        private bool IsActivated
        {
            get { return _isActivated; }
            set
            {
                if (_isActivated != value)
                {
                    _isActivated = value;
                    if (_isActivated)
                    {
                        this.activateSTSEffect();
                        _isActivated = true;
                    }
                    else
                    {
                        this.deactivateSTSEffect();
                        _isActivated = false;
                    }
                }
            }
        }

        public bool toggleActivation = false;


        void Update()
        {
            IsActivated = toggleActivation;
        }

        protected override void OnDestroyImpl()
        {
        }

        protected override void OnStart()
        {
        }


    }
}