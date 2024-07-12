using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextLogFade : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private bool fadeOut, fadeIn;
    public bool isActive;
    public float fadeSpeed;


    public GameObject TextLog;
  public TextLogOpen textLogOpen;
    //   
    /*  public CanvasGroup uiElement;


      public IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float lerpTime = 1)
      {
          while (true)
          {
              yield return new WaitForEndOfFrame();
          }



      }
      //
      */


    public void Update()
    {
        textLogOpen.OpenPanel();

        isActive = true;
        
            if (isActive)
        {
            FadeOutObject();
        }

        if (fadeOut)
        {
            Color objectColor = TextLog.GetComponent<Image>().material.color;
            float fadeAmount = objectColor.a - (fadeSpeed * Time.deltaTime);

            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            TextLog.GetComponent<Image>().material.color = objectColor;

            if (objectColor.a <= 0)
            {
                fadeOut = false;
            }

        }
    }

    public void FadeOutObject()
    {
        fadeOut = true;


    }
}
