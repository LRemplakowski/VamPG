using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IInteractable
{
    
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

    GameObject TargetedBy
    {
        get;
        set;
    }

    Transform InteractionTransform
    {
        get;
        set;
    }

    bool Interacted
    {
        get;
        set;
    }

    void Interact();

    void OnDrawGizmosSelected();
}
