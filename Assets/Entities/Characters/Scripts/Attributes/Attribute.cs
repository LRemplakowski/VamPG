using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attribute : BaseStat
{
    [SerializeField, Range(1, 10)]
    private int baseValue = 1;
    [SerializeField]
    private AttributeType attributeType;

    protected override void SetBaseValueImpl(int value)
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
}
