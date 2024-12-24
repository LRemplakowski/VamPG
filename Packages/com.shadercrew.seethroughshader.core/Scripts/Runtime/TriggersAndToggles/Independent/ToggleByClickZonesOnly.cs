using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShaderCrew.SeeThroughShader
{
    [AddComponentMenu(Strings.COMPONENTMENU_TOGGLE_BY_CLICK_ZONES_ONLY)]
    public class ToggleByClickZonesOnly : MonoBehaviour
    {

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


                RaycastHit raycastHit;
                if (Physics.Raycast(ray, out raycastHit, 100f))
                {
                    if (raycastHit.transform != null)
                    {
                        OnGameObjectClicked(raycastHit.transform.gameObject);
                    }
                }
            }
        }

        public void OnGameObjectClicked(GameObject gameObject)
        {
            if (gameObject.GetComponent<SeeThroughShaderZone>())
            {
                SeeThroughShaderZone seeThroughShaderZone = gameObject.GetComponent<SeeThroughShaderZone>();
                if (seeThroughShaderZone.enabled)
                {
                    seeThroughShaderZone.toggleZoneActivation();
                }
                
            }
        }




    }


}