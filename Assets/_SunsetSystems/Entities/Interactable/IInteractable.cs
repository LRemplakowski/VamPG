using System.Collections.Generic;
using SunsetSystems.Entities.Characters.Actions;
using SunsetSystems.Entities.Characters.Interfaces;
using SunsetSystems.Entities.Interactable;
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

    List<IInteractionHandler> InteractionHandlers
    {
        get; 
        set;
    }

    void Interact();

    void OnDrawGizmosSelected();
}
