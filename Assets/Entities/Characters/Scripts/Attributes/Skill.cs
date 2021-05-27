using UnityEngine;

[System.Serializable]
public class Skill : BaseStat
{
    [SerializeField]
    private SkillType skillType;

    public Skill(SkillType skillType)
    {
        this.skillType = skillType;
    }

    public override int GetValue()
    {
        return baseValue;
    }
}
