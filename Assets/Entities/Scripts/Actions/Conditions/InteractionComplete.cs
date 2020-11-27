using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionComplete : Condition
{
    IInteractable target;
    public InteractionComplete(IInteractable target)
    {
        this.target = target;
    }

    public override bool IsMet()
    {
        if(target.Interacted == true)
        {
            target.Interacted = false;
            return true;
        }
        return false;
    }
}
