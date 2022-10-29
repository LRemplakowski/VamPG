using SunsetSystems.Entities.Characters;
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

    Creature TargetedBy
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
