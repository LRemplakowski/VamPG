using UnityEngine;
using System.Collections;

//TMP class
public class HairDesignerTmp : MonoBehaviour
{ 
    void Awake()
    {
        if (Application.isPlaying)
            Destroy(gameObject);
    }
}
