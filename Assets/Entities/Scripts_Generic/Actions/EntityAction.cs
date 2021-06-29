using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityAction
{
    protected abstract Creature Owner { get; set; }
    protected List<Condition> conditions = new List<Condition>();
    
    /// <summary>
    /// Rozpoczęcie akcji. Wywoływane kiedy akcja trafia na początek kolejki akcji.
    /// </summary>
    public abstract void Begin();

    /// <summary>
    /// Przerwanie akcji. Wywoływane kiedy warunki ukończenia akcji zostaną spełnione lub zewnętrznie przez inną klasę.
    /// </summary>
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
