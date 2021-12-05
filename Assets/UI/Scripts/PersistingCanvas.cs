using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.Singleton;

public class PersistingCanvas : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }
}
