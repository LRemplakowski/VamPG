using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attribute : BaseStat
{
    [SerializeField]
    private AttributeType attributeType;

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
