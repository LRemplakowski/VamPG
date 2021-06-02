using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityAction
{
    protected abstract Creature Owner { get; set; }
    protected List<Condition> conditions = new List<Condition>();

    public abstract void Begin();

    public abstract void Abort();

    public virtual bool IsFinished()
    {
        if (conditions.Count == 0)
        {
            Debug.LogError("Aborting action, no conditions present");
            this.Abort();
            return true;
        }
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

    public override string ToString()
    {
        string action = "EntityAction: " + this.GetType() + "\nConditions:";
        foreach (Condition c in conditions)
        {
            action += "\n" + c;
        }
        return action;
    }
}
