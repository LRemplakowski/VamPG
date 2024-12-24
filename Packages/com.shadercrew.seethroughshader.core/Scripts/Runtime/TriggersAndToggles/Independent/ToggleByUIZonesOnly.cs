using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShaderCrew.SeeThroughShader
{
    [AddComponentMenu(Strings.COMPONENTMENU_TOGGLE_BY_UI_ZONES_ONLY)]
    public class ToggleByUIZonesOnly : MonoBehaviour
    {

        public List<ButtonToZones>  buttonToZones = new List<ButtonToZones>();

        bool initialized = false;

        public bool toggleAllOnStart = false;

        // Start is called before the first frame update

        void Start()
        {
            if (this.isActiveAndEnabled)
            {
                InitializeToggle();

                if(toggleAllOnStart)
                {
                    foreach (ButtonToZones buttonToZones in buttonToZones)
                    {
                        if(buttonToZones != null && buttonToZones.zoneList != null)
                        {
                            foreach (SeeThroughShaderZone zones in buttonToZones.zoneList)
                            {
                                if (zones != null && zones.enabled)
                                {
                                    zones.toggleZoneActivation();
                                }
                            }
                        }

                    }
                }
            }
        }

        private void InitializeToggle()
        {

            if (!initialized)
            {
                if(buttonToZones != null)
                {
                    foreach (ButtonToZones buttonToZones in buttonToZones)
                    {
                        if (buttonToZones != null && buttonToZones.button != null && buttonToZones.zoneList != null)
                        {
                            buttonToZones.button.onClick.AddListener(delegate { UIButtonOnClick(buttonToZones.zoneList); });
                        }

                    }
                }
                initialized = true;
            }
        }


        private void UIButtonOnClick(List<SeeThroughShaderZone> seeThroughShaderZoneList)
        {
            foreach (SeeThroughShaderZone zone in seeThroughShaderZoneList)
            {
                if(zone != null && zone.enabled)
                {
                    zone.toggleZoneActivation();
                }
            }
        }

    }

    [System.Serializable]
    public class ButtonToZones
    {
        public Button button;
        public List<SeeThroughShaderZone> zoneList;
    }
}