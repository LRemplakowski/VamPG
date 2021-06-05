using UnityEngine;

[System.Serializable]
public class Skill : BaseStat
{
    [SerializeField, Range(0,5)]
    private int baseValue;
    [SerializeField]
    private SkillType skillType;

    protected override void SetBaseValueImpl(int value)
    {
        this.baseValue = value;
    }

    public Skill(SkillType skillType)
    {
        this.skillType = skillType;
    }

    public override int GetValue()
    {
        return baseValue;
    }

    public SkillType GetSkillType()
    {
        return this.skillType;
    }
}
