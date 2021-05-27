using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Tracker : BaseStat
{
    [SerializeField]
    private int currentValue;
    [SerializeField]
    private TrackerType trackerType;

    public delegate void OnTrackerNegativeOrZero();
    public OnTrackerNegativeOrZero onTrackerNegativeOrZero;

    public Tracker(TrackerType trackerType)
    {
        this.trackerType = trackerType;
    }

    public override int GetValue()
    {
        int finalValue = baseValue;
        foreach (Modifier m in modifiers)
        {
            finalValue += m.Value;
        }
        return finalValue;  
    }

    public int GetCurrentValue()
    {
        return currentValue;
    }

    public void SetCurrentValue(int newValue)
    {
        currentValue = newValue;
        if (currentValue <= 0 && onTrackerNegativeOrZero != null)
        {
            onTrackerNegativeOrZero.Invoke();
        }
    }
}
