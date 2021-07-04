using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Discipline : BaseStat
{
    [SerializeField, Range(0, 5)]
    private int baseValue = 0;

    [SerializeField, ReadOnly]
    private DisciplineType disciplineType;

    [SerializeField]
    private DisciplinePower[] knownPowers = new DisciplinePower[5];

    public DisciplinePower GetPower(int index)
    {
        DisciplinePower p;
        try
        {
            p = knownPowers[index];
            return p ? p : ScriptableObject.CreateInstance<DisciplinePower>();
        }
        catch (IndexOutOfRangeException e)
        {
            return ScriptableObject.CreateInstance<DisciplinePower>();
        }
    }

    public List<DisciplinePower> GetKnownPowers() => new List<DisciplinePower>(knownPowers);

    public Discipline(DisciplineType disciplineType)
    {
        this.disciplineType = disciplineType;
    }

    public override int GetValue()
    {
        int value = baseValue;
        modifiers.ForEach(m => value += m.Value);
        return value;
    }

    public override int GetValue(bool includeModifiers)
    {
        int value = baseValue;
        if (includeModifiers)
            modifiers.ForEach(m => value += m.Value);
        return value;
    }

    public override int GetValue(List<ModifierType> modifierTypes)
    {
        int finalValue = baseValue;
        modifiers.ForEach(m => finalValue += modifierTypes.Contains(m.Type) ? m.Value : 0);
        return finalValue;
    }

    protected override void SetValueImpl(int value)
    {
        this.baseValue = value;
    }

    public DisciplineType GetDisciplineType()
    {
        return disciplineType;
    }
}