using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Discipline : BaseStat
{
    [SerializeField, Range(0, 5)]
    private int baseValue = 0;

    [SerializeField, ReadOnly]
    private DisciplineType disciplineType = DisciplineType.Invalid;

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
        catch (ArgumentOutOfRangeException e)
        {
            Debug.LogException(e);
            return ScriptableObject.CreateInstance<DisciplinePower>();
        }
    }

    public List<DisciplinePower> GetKnownPowers() => new List<DisciplinePower>(knownPowers);

    public Discipline(DisciplineType disciplineType)
    {
        this.disciplineType = disciplineType;
    }

    public override int GetValue(ModifierType modifierTypesFlag)
    {
        int finalValue = baseValue;
        Modifiers.ForEach(m => finalValue += (modifierTypesFlag & m.Type) > 0 ? m.Value : 0);
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