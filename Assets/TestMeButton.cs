using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMeButton : MonoBehaviour
{

    [SerializeField]

    private GameObject buttonTemplate;

    GameObject button;

    void Start()
    {
        /*       // for (int i = 1; 1 <= 20; i++)
               // {



               //}
               */

        GameObject button = Instantiate(buttonTemplate) as GameObject;
        button.SetActive(true);


    }


    public void ButtonClicked(string myTextString)
    {
        Debug.Log(myTextString);

        button.GetComponent<textListButton>().SetText("You have acquired an object. ");//+ i);

        button.transform.SetParent(buttonTemplate.transform.parent, false);
    }
}
