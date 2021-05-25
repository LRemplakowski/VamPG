using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attribute : BaseStat
{
    public override int GetValue()
    {
        int finalValue = baseValue;
        modifiers.ForEach(m => finalValue += m.Value);
        return finalValue;
    }
}
