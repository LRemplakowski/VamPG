using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class Tracker : BaseStat
{
    [SerializeField]
    private int _maxValue = 1;
    public int MaxValue => _maxValue;
    [field: SerializeField]
    public int SuperficialDamage { get; set; } 
    [field: SerializeField]
    public int AggravatedDamage { get; set; }

    public override string Name => Enum.GetName(typeof(TrackerType), _trackerType);

    [SerializeField, ReadOnly]
    private TrackerType _trackerType;

    protected override void SetValueImpl(int value)
    {
        this._maxValue = value;
    }

    public Tracker(Tracker existing) : base(existing)
    {
        this._maxValue = existing.MaxValue;
        this.SuperficialDamage = existing.SuperficialDamage;
        this.AggravatedDamage = existing.AggravatedDamage;
        this._trackerType = existing._trackerType;
    }

    public Tracker(TrackerType trackerType)
    {
        this._trackerType = trackerType;
    }

    public override int GetValue(ModifierType modifierTypesFlag)
    {
        int finalValue = _maxValue - SuperficialDamage - AggravatedDamage;
        Modifiers?.ForEach(m => finalValue += (modifierTypesFlag & m.Type) > 0 ? m.Value : 0);
        return finalValue;
    }

    public TrackerType GetTrackerType()
    {
        return this._trackerType;
    }
}
