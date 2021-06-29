using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attribute : BaseStat
{
    [SerializeField, Range(1, 10)]
    private int baseValue = 1;
    [SerializeField, ReadOnly]
    private AttributeType attributeType;

    protected override void SetValueImpl(int value)
    {
        this.baseValue = value;
    }

    public Attribute(AttributeType attributeType)
    {
        this.attributeType = attributeType;
    }
    public override int GetValue()
    {
        int finalValue = baseValue;
        modifiers.ForEach(m => finalValue += m.Value);
        return finalValue;
    }

    public override int GetValue(bool includeModifiers)
    {
        int finalValue = baseValue;
        if (includeModifiers)
            modifiers.ForEach(m => finalValue += m.Value);
        return finalValue;
    }

    public override int GetValue(List<ModifierType> modifierTypes)
    {
        int finalValue = baseValue;
        modifiers.ForEach(m => finalValue += modifierTypes.Contains(m.Type) ? m.Value : 0);
        return finalValue;
    }

    public AttributeType GetAttributeType()
    {
        return this.attributeType;
    }
}
