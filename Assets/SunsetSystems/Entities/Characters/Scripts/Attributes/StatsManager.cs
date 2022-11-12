using Entities.Characters.Data;
using System.Collections.Generic;
using UnityEngine;
using SunsetSystems.Dice;
using NaughtyAttributes;
using SunsetSystems.Entities.Data;
using System.Linq;
using SunsetSystems.Spellbook;

namespace SunsetSystems.Entities.Characters
{
    public class StatsManager : MonoBehaviour
    {
        [field: SerializeField]
        public StatsData Data { get; private set; } = new();

        public Tracker Health => Data.trackers.GetTracker(TrackerType.Health);
        public Tracker Willpower => Data.trackers.GetTracker(TrackerType.Willpower);
        public Tracker Hunger => Data.trackers.GetTracker(TrackerType.Hunger);
        public Tracker Humanity => Data.trackers.GetTracker(TrackerType.Humanity);

        public void Initialize(StatsData data)
        {
            this.Data = data;
        }

        public void TakeDamage(int damage)
        {
            Debug.Log("Taking damage! " + damage);
        }

        public virtual void Die()
        {
            Debug.Log("Character died!");
        }

        public int GetCombatSpeed()
        {
            CreatureAttribute dexterity = Data.attributes.GetAttribute(AttributeType.Dexterity);
            Skill athletics = Data.skills.GetSkill(SkillType.Athletics);
            if (dexterity == null && athletics == null)
                return 0;
            else
                return dexterity.GetValue() + athletics.GetValue();
        }

        public int GetInitiative()
        {
            return Data.attributes.GetAttribute(AttributeType.Dexterity).GetValue();
        }

        public float GetWeaponMaxRange()
        {
            return 10.0f;
        }

        public float GetWeaponEffectiveRange()
        {
            return 5.0f;
        }

        public bool IsAlive()
        {
            return true;
        }

        public AttributeSkillPool GetDefensePool()
        {
            return new AttributeSkillPool(Data.attributes.GetAttribute(AttributeType.Dexterity), Data.skills.GetSkill(SkillType.Athletics));
        }

        public AttributeSkillPool GetAttackPool()
        {
            return new AttributeSkillPool(Data.attributes.GetAttribute(GetWeaponAttribute()), Data.skills.GetSkill(GetWeaponSkill()));
        }

        private AttributeType GetWeaponAttribute()
        {
            return AttributeType.Composure;
        }

        private SkillType GetWeaponSkill()
        {
            return SkillType.Firearms;
        }

        public DisciplinePower GetDisciplinePower(string scriptName)
        {
            foreach (Discipline discipline in Data.disciplines.GetDisciplines())
            {
                DisciplinePower power = discipline.GetKnownPowers().Find(p => p.ScriptName.Equals(scriptName));
                if (power != null)
                    return power;
            }
            return null;
        }

        public Outcome GetSkillRoll(AttributeType attribute, SkillType skill, bool useHunger = false)
        {
            CreatureAttribute a = Data.attributes.GetAttribute(attribute);
            Skill s = Data.skills.GetSkill(skill);
            int normalDice = a.GetValue() + s.GetValue();
            int hungerDice = 0;
            if (useHunger)
            {
                hungerDice = Hunger.GetValue();
                normalDice = hungerDice <= normalDice ? normalDice - hungerDice : 0;
            }

            return Roll.d10(normalDice, hungerDice);
        }

        public Outcome GetSkillRoll(AttributeType attribute, SkillType skill, int dc, bool useHunger = false)
        {
            CreatureAttribute a = Data.attributes.GetAttribute(attribute);
            Skill s = Data.skills.GetSkill(skill);
            int normalDice = a.GetValue() + s.GetValue();
            int hungerDice = 0;
            if (useHunger)
            {
                hungerDice = Hunger.GetValue();
                normalDice = hungerDice <= normalDice ? normalDice - hungerDice : 0;
            }

            return Roll.d10(normalDice, hungerDice, dc);
        }

        public Outcome GetAttackRoll(int dc, bool useHunger)
        {
            AttributeType weaponAttribute = GetWeaponAttribute();
            SkillType weaponSkill = GetWeaponSkill();
            return GetSkillRoll(weaponAttribute, weaponSkill, dc, useHunger);
        }

        public List<CreatureAttribute> GetAttributes()
        {
            return Data.attributes.GetAttributeList();
        }

        internal HealthData GetHealthData()
        {
            HealthData.HealthDataBuilder builder = new(Data.trackers.GetTracker(TrackerType.Health).GetValue());
            return builder.Create();
        }
    }
}
