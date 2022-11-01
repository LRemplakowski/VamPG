using Adnc.Utility;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CreatureAttribute : BaseStat
{
    [SerializeField, Range(1, 10)]
    private int baseValue = 1;
    [SerializeField, ReadOnly]
    private AttributeType attributeType;

    public int Value { get => GetValue(); }
    public AttributeType AttributeType { get => attributeType; }

    protected override void SetValueImpl(int value)
    {
        this.baseValue = value;
    }

    public CreatureAttribute(AttributeType attributeType)
    {
        this.attributeType = attributeType;
    }

    public override int GetValue(ModifierType modifierTypesFlag)
    {
        int finalValue = baseValue;
        Modifiers?.ForEach(m => finalValue += (modifierTypesFlag & m.Type) > 0 ? m.Value : 0);
        return finalValue;
    }

    public AttributeType GetAttributeType()
    {
        return this.attributeType;
    }
}
