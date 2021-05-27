using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseStat
{
    [SerializeField]
    protected int baseValue;

    public delegate void OnValueChange();
    public OnValueChange onValueChange;

    protected List<Modifier> modifiers = new List<Modifier>();

    public void SetBaseValue(int value)
    {
        baseValue = value;
        if (onValueChange != null)
            onValueChange.Invoke();
    }

    public abstract int GetValue();

    public virtual void AddModifier(int value, ModifierType type)
    {
        modifiers.Add(new Modifier(value, type));
        if (onValueChange != null)
            onValueChange.Invoke();
    }

    public virtual void AddModifier(Modifier modifier)
    {
        modifiers.Add(modifier);
        if (onValueChange != null)
            onValueChange.Invoke();
    }

    public virtual void RemoveModifiersOfType(ModifierType type)
    {
        modifiers.RemoveAll(m => m.Type.Equals(type));
        if (onValueChange != null)
            onValueChange.Invoke();
    }

    public virtual void RemoveModifier(Modifier modifier)
    {
        modifiers.Remove(modifier);
        if (onValueChange != null)
            onValueChange.Invoke();
    }
}
