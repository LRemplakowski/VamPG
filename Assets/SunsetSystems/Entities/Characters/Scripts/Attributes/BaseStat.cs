using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseStat
{
    public delegate void OnValueChange();
    public OnValueChange onValueChange;

    protected List<Modifier> Modifiers { get; } = new();

    public BaseStat(BaseStat existing)
    {
        Modifiers = new();
        existing.Modifiers.ForEach(m  => Modifiers.Add(new(m.Value, m.Type, m.Name)));
    }

    public BaseStat()
    {

    }

    public virtual void SetValue(int value)
    {
        SetValueImpl(value);
        if (onValueChange != null)
            onValueChange.Invoke();
    }

    protected abstract void SetValueImpl(int value);

    public int GetValue()
    {
        return GetValue(true);
    }

    public int GetValue(bool includeModifiers)
    {
        if (includeModifiers)
            return GetValue(ModifierType.ALL);
        else
            return GetValue(ModifierType.NONE);
    }

    public abstract int GetValue(ModifierType modifierTypesFlag);

    public virtual void AddModifier(int value, ModifierType type, string name)
    {
        Modifiers.Add(new Modifier(value, type, name));
        if (onValueChange != null)
            onValueChange.Invoke();
    }

    public virtual void AddModifier(Modifier modifier)
    {
        Modifiers.Add(modifier);
        if (onValueChange != null)
            onValueChange.Invoke();
    }

    public virtual void AddModifiers(List<Modifier> modifiers)
    {
        modifiers.ForEach(m => this.Modifiers.Add(m));
    }

    public virtual void RemoveModifiersOfType(ModifierType type)
    {
        Modifiers.RemoveAll(m => (m.Type & ModifierType.ALL) > 0);
        if (onValueChange != null)
            onValueChange.Invoke();
    }

    public virtual void RemoveModifier(Modifier modifier)
    {
        Modifiers.Remove(modifier);
        if (onValueChange != null)
            onValueChange.Invoke();
    }

    public virtual List<Modifier> GetModifiers()
    {
        return Modifiers;
    }
}
