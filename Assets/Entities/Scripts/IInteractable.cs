using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IInteractable
{
    [SerializeField]
    GameObject HoverHighlight
    {
        get;
        set;
    }

    bool IsHoveredOver
    {
        get;
        set;
    }

    float InteractionDistance
    {
        get;
        set;
    }

    void Interact();
}
