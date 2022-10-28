using Entities.Characters.Data;
using System.Collections.Generic;
using UnityEngine;
using SunsetSystems.Dice;
using NaughtyAttributes;
using SunsetSystems.Entities.Data;

namespace SunsetSystems.Entities.Characters
{
    public class StatsManager : MonoBehaviour
    {
        [SerializeField]
        protected StatsConfig _characterStats;
        [field: SerializeField]
        public StatsData Data { get; private set; }

        public Tracker Health => Data.trackers.GetTracker(TrackerType.Health);
        public Tracker Willpower => Data.trackers.GetTracker(TrackerType.Willpower);
        public Tracker Hunger => Data.trackers.GetTracker(TrackerType.Hunger);
        public Tracker Humanity => Data.trackers.GetTracker(TrackerType.Humanity);

        private void Awake()
        {
            if (!_characterStats)
                _characterStats = ScriptableObject.CreateInstance(typeof(StatsConfig)) as StatsConfig;
        }

        public void Initialize(StatsData data)
        {
            this.Data = data;
        }

        public void TakeDamage(int damage)
        {
            //int newHealth = Health - damage;
            //Debug.Log(gameObject.name + " takes " + damage + " damage!" + "\nCurrent health: " + Health + "\nHealth after attack: " + newHealth);
            //Health = newHealth < 0 ? 0 : newHealth;
            //if (Health <= 0)
            //    Die();
        }

        public virtual void Die()
        {
            Debug.Log("Character died!");
        }

        public int GetCombatSpeed()
        {
            CreatureAttribute dexterity = _characterStats.GetAttribute(AttributeType.Dexterity);
            Skill athletics = _characterStats.GetSkill(SkillType.Athletics);
            if (dexterity.GetAttributeType() != AttributeType.Invalid && athletics.GetSkillType() != SkillType.Invalid)
                return (dexterity.GetValue() + athletics.GetValue());
            else
                return 0;
        }

        public int GetInitiative()
        {
            return 0;
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
            return Health.GetValue() > 0;
        }

        public AttributeSkillPool GetDefensePool()
        {
            return new AttributeSkillPool(_characterStats.GetAttribute(AttributeType.Dexterity), _characterStats.GetSkill(SkillType.Athletics));
        }

        public AttributeSkillPool GetAttackPool()
        {
            return new AttributeSkillPool(_characterStats.GetAttribute(GetWeaponAttribute()), _characterStats.GetSkill(GetWeaponSkill()));
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
            return _characterStats.GetDisciplinePower(scriptName);
        }

        public Outcome GetSkillRoll(AttributeType attribute, SkillType skill, bool useHunger = false)
        {
            CreatureAttribute a = _characterStats.GetAttribute(attribute);
            Skill s = _characterStats.GetSkill(skill);
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
            CreatureAttribute a = _characterStats.GetAttribute(attribute);
            Skill s = _characterStats.GetSkill(skill);
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
            return _characterStats.GetAttributes();
        }

        internal HealthData GetHealthData()
        {
            HealthData.HealthDataBuilder builder = new(Data.trackers.GetTracker(TrackerType.Health).GetValue());
            return builder.Create();
        }
    }
}
