using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextLogOpen : MonoBehaviour
{
    public GameObject TextLog;

    public TextLogFade textLogFade;

   
public void OpenPanel()
    {
        if(TextLog != null)
        {
            TextLog.SetActive(true);

        //  textLogFade.Update();
        }

    }


 

    
 
}
