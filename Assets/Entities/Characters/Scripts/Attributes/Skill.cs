using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Skill : BaseStat
{
    [SerializeField, Range(0,5)]
    private int baseValue;
    [SerializeField, ReadOnly]
    private SkillType skillType;

    protected override void SetValueImpl(int value)
    {
        this.baseValue = value;
    }

    public Skill(SkillType skillType)
    {
        this.skillType = skillType;
    }

    public override int GetValue()
    {
        int finalValue = baseValue;
        modifiers.ForEach(m => finalValue += m.Value);
        return finalValue;
    }

    public override int GetValue(bool includeModifiers)
    {
        int finalValue = baseValue;
        if (includeModifiers)
            modifiers.ForEach(m => finalValue += m.Value);
        return finalValue;
    }

    public override int GetValue(List<ModifierType> modifierTypes)
    {
        int finalValue = baseValue;
        modifiers.ForEach(m => finalValue += modifierTypes.Contains(m.Type) ? m.Value : 0);
        return finalValue;
    }

    public SkillType GetSkillType()
    {
        return this.skillType;
    }
}
