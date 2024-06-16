using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class textListButton : MonoBehaviour
{

    [SerializeField]

    private TextMeshProUGUI myText;

    [SerializeField]

    //  private textListControl textControl;

    // private string myTextString;

    private TestMeButton testTheButton;
    private string myNewTextString;


    public void SetText(string textString)
    {
        //     myTextString = textString;
        myNewTextString = textString;

        myText.text = textString;
    }

    public void OnClick()
    {

        //  textControl.ButtonClicked(myTextString);

        testTheButton.ButtonClicked(myNewTextString);
     //  button.GetComponent<textListButton>().SetText("You have acquired an object. ");//+ i);

    }
}
