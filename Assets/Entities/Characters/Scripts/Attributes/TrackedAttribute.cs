using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class TrackedAttribute : Attribute
{
    [SerializeField, ReadOnly]
    private int currentValue;
    
    public int GetCurrentValue()
    {
        return currentValue;
    }

    public void SetCurrent(int value)
    {
        currentValue = value;
        currentValue = Mathf.Clamp(currentValue, 0, GetValue());
    }
}
