using System.Collections.Generic;
using SunsetSystems.Entities.Characters.Actions;
using SunsetSystems.Entities.Interactable;
using UnityEngine;

public interface IInteractable
{
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

    IActionPerformer TargetedBy
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

    bool Interactable
    {
        get;
        set;
    }

    void Interact();
}
