using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inspect : ContextAction
{
    public override void OnClick()
    {
        Debug.Log("Inspecting item");
    }
}
