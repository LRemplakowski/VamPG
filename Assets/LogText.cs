using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogText : MonoBehaviour
{

    public static LogText _instance;
    public static LogText instance {  get { return _instance; } }
    //  private struct log;
    private Text logText;


    private void Awake()
    {
        logText = FindObjectOfType<Text>();
        if (_instance != null && _instance != this)
        {
            
        }
        else
        {
            _instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        logText.text = logText.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    


}
