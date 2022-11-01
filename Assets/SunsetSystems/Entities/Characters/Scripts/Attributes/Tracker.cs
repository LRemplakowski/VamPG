using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class Tracker : BaseStat
{
    [SerializeField]
    private int maxValue = 1;
    [SerializeField]
    private int superficialDamage, aggravatedDamage;
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

    public override int GetValue(ModifierType modifierTypesFlag)
    {
        int finalValue = maxValue - superficialDamage - aggravatedDamage;
        Modifiers?.ForEach(m => finalValue += (modifierTypesFlag & m.Type) > 0 ? m.Value : 0);
        return finalValue;
    }

    public TrackerType GetTrackerType()
    {
        return this.trackerType;
    }
}
