using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Tracker : BaseStat
{
    [SerializeField]
    private int maxValue = 1;
    [SerializeField, ReadOnly]
    private TrackerType trackerType;

    protected override void SetValueImpl(int value)
    {
        this.maxValue = value;
    }

    public Tracker(TrackerType trackerType)
    {
        this.trackerType = trackerType;
    }

    public override int GetValue()
    {
        int finalValue = maxValue;
        modifiers.ForEach(m => finalValue += m.Value);
        return finalValue;
    }

    public override int GetValue(bool includeModifiers)
    {
        int finalValue = maxValue;
        if (includeModifiers)
            modifiers.ForEach(m => finalValue += m.Value);
        return finalValue;
    }

    public override int GetValue(List<ModifierType> modifierTypes)
    {
        int finalValue = maxValue;
        modifiers.ForEach(m => finalValue += modifierTypes.Contains(m.Type) ? m.Value : 0);
        return finalValue;
    }

    public TrackerType GetTrackerType()
    {
        return this.trackerType;
    }
}
