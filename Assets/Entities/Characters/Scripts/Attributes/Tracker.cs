using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Tracker : BaseStat
{
    [SerializeField, Range(1, 100)]
    private int baseValue = 1;
    [SerializeField]
    private int currentValue;
    [SerializeField]
    private TrackerType trackerType;

    public delegate void OnTrackerNegativeOrZero();
    public OnTrackerNegativeOrZero onTrackerNegativeOrZero;

    protected override void SetBaseValueImpl(int value)
    {
        this.baseValue = value;
    }

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

    public TrackerType GetTrackerType()
    {
        return this.trackerType;
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
