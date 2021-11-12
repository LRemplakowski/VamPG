using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaEntryPoint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameManager.GetPlayer().ForceCreatureToPosition(this.transform.position);
        CameraControlScript.Instance.Initialize();
    }
}
