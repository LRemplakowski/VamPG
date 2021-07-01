using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseStat
{
    public delegate void OnValueChange();
    public OnValueChange onValueChange;

    protected List<Modifier> modifiers = new List<Modifier>();

    public virtual void SetValue(int value)
    {
        SetValueImpl(value);
        if (onValueChange != null)
            onValueChange.Invoke();
    }

    protected abstract void SetValueImpl(int value);

    public abstract int GetValue();

    public abstract int GetValue(bool includeModifiers);

    public abstract int GetValue(List<ModifierType> modifierTypes);

    public virtual void AddModifier(int value, ModifierType type, string name)
    {
        modifiers.Add(new Modifier(value, type, name));
        if (onValueChange != null)
            onValueChange.Invoke();
    }

    public virtual void AddModifier(Modifier modifier)
    {
        modifiers.Add(modifier);
        if (onValueChange != null)
            onValueChange.Invoke();
    }

    public virtual void AddModifiers(List<Modifier> modifiers)
    {
        modifiers.ForEach(m => this.modifiers.Add(m));
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

    public virtual List<Modifier> GetModifiers()
    {
        return modifiers;
    }
}
