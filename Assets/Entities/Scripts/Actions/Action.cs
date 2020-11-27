using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Action
{
    protected Creature owner;
    protected List<Condition> conditions = new List<Condition>();

    public abstract void Begin();

    public virtual bool IsFinished()
    {
        if (conditions.Count == 0) return true;
        foreach(Condition c in conditions)
        {
            if(c.IsMet())
            {
                continue;
            }
            else
            {
                return false;
            }
        }
        return true;
    }
}
