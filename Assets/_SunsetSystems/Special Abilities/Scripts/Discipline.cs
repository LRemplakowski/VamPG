using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Spellbook
{
    [System.Serializable]
    public class Discipline : BaseStat
    {
        [SerializeField, Range(0, 5)]
        private int baseValue = 0;

        [SerializeField, ReadOnly]
        private DisciplineType disciplineType = DisciplineType.Invalid;

        public override string Name => disciplineType.ToString();

        [SerializeField]
        private List<DisciplinePower> knownPowers = new();

        public Discipline(Discipline existing) : base(existing)
        {
            knownPowers = new(existing.knownPowers);
            disciplineType = existing.disciplineType;
        }

        public DisciplinePower GetPower(int index)
        {
            try
            {
                return knownPowers[index];
            }
            catch (ArgumentOutOfRangeException e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public List<DisciplinePower> GetKnownPowers() => knownPowers;

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
}