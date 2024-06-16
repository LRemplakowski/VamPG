using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextLogItem : MonoBehaviour
{
    
    public void SetText(string myText, Color myColor)
    {
        GetComponent<TMP_Text>().text = myText;
        GetComponent<TMP_Text>().color = myColor;
    
    
    }







}
