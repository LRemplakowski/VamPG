using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityAction
{
    protected Creature owner;
    protected List<Condition> conditions = new List<Condition>();

    public abstract void Begin();

    public abstract void Abort();

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
        this.Abort();
        return true;
    }
}
