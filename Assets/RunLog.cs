using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunLog : MonoBehaviour
{

    [SerializeField]
    private string myText;
    [SerializeField]
    private Color myColor;

    [SerializeField]
    private TextLogControl logControl;

    public void LogText()
    {
        logControl.LogText(myText, myColor);

    }

}
