using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShaderCrew.SeeThroughShader
{
    [AddComponentMenu(Strings.COMPONENTMENU_TOGGLE_BY_UI)]
    public class ToggleByUI : ToogleByWhateverAbstract
    {

        public Button button;
        bool activated = false;

        protected override void OnStart()
        {
            if (button != null)
            {
                button.onClick.AddListener(delegate { UIButtonOnClick(); });
            }
        }


        private void UIButtonOnClick()
        {
            if (!activated)
            {
                activateSTSEffect();
                activated = true;
            }
            else
            {
                deactivateSTSEffect();
                activated = false;
            }
        }


        protected override void OnDestroyImpl(){}
    }
}