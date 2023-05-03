using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems
{
    public class ActionBusyUI : MonoBehaviour
    {
        
        private void Start(){
            UnitSystem.Instance.OnBusyChanged += UnitSystem_OnBusyChanged;

            Hide();
        }
        
        private void Show(){
            gameObject.SetActive(true);
        }

        private void Hide(){
            gameObject.SetActive(false);
        }

        private void UnitSystem_OnBusyChanged(object sender, bool isBusy){
            if (isBusy){
                Show();
            }else{
                Hide();
            }
        }
    }
}
