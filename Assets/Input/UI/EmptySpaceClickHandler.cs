using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptySpaceClickHandler : MonoBehaviour
{
    public void OnClick()
    {
        CustomContextMenu.ClearContextMenu();
    }
}
