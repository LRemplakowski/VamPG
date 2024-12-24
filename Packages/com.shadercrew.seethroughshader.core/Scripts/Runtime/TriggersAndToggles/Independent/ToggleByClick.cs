using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShaderCrew.SeeThroughShader
{
    [AddComponentMenu(Strings.COMPONENTMENU_TOGGLE_BY_CLICK)]
    public class ToggleByClick : ToogleByWhateverAbstract
    {
        bool activated = false;


        RaycastHit[] hits = new RaycastHit[100];



        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


                int hitCount = Physics.RaycastNonAlloc(ray, hits);

 

                for (int i = 0; i < hitCount; i++)
                {
                    if (hits[i].collider != null && hits[i].transform != null)
                    {
                        OnGameObjectClicked(hits[i].transform.gameObject);
                    }
                }
            }
        }

        public void OnGameObjectClicked(GameObject gameObject)
        {
            if (this.gameObject == gameObject)
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
        }


        protected override void OnStart()
        {
        }


        protected override void OnDestroyImpl()
        {
        }
    }


}