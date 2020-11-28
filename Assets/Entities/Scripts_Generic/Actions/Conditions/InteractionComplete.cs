using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionComplete : Condition
{
    readonly IInteractable target;
    public InteractionComplete(IInteractable target)
    {
        this.target = target;
    }

    public override bool IsMet()
    {
        if(target.Interacted == true)
        {
            return true;
        }
        return false;
    }
}
