using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Skill : BaseStat
{
    [SerializeField, Range(0, 5)]
    private int baseValue;
    [SerializeField, ReadOnly]
    private SkillType skillType;

    public override string Name => skillType.ToString();

    public float Value { get => GetValue(); }
    public SkillType SkillType { get => skillType; }

    public Skill(Skill existing) : base(existing)
    {
        baseValue = existing.baseValue;
        skillType = existing.skillType;
    }

    public Skill() : this(SkillType.Invalid)
    {

    }

    protected override void SetValueImpl(int value)
    {
        this.baseValue = value;
    }

    public Skill(SkillType skillType)
    {
        this.skillType = skillType;
    }

    public override int GetValue(ModifierType modifierTypesFlag)
    {
        int finalValue = baseValue;
        Modifiers?.ForEach(m => finalValue += (modifierTypesFlag & m.Type) > 0 ? m.Value : 0);
        return finalValue;
    }

    public SkillType GetSkillType()
    {
        return this.skillType;
    }
}
