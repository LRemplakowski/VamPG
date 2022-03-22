using Entities.Characters.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SunsetSystems.Dice;

namespace Entities.Characters
{
    public class StatsManager : ExposableMonobehaviour
    {
        [SerializeField]
        protected CharacterStats _characterStats;
        public CharacterStats Stats 
        { 
            get => _characterStats; 
            internal set
            {
                _characterStats = value;
                InitializeTrackers();
            }
        }

        [SerializeField, ReadOnly]
        protected Creature owner;

        [SerializeField, ReadOnly]
        private int _health;
        public int Health { get => _health; private set => _health = value; }
        [SerializeField, ReadOnly]
        private int _willpower;
        public int Willpower { get => _willpower; private set => _willpower = value; }
        [SerializeField, ReadOnly]
        private int _humanity;
        public int Humanity { get => _humanity; private set => _humanity = value; }
        [SerializeField, ReadOnly]
        private int _hunger;
        public int Hunger { get => _hunger; private set => _hunger = value; }

        private void Awake()
        {
            owner = GetComponentInParent<Creature>();
            if (!_characterStats)
                _characterStats = ScriptableObject.CreateInstance(typeof(CharacterStats)) as CharacterStats;
            _characterStats = CharacterStats.CopyAssetInstance(_characterStats);
            InitializeTrackers();
        }

        private void InitializeTrackers()
        {
            _characterStats.GetTracker(TrackerType.Health).SetValue(_characterStats.GetAttribute(AttributeType.Stamina).GetValue() + 3);
            Health = _characterStats.GetTracker(TrackerType.Health).GetValue();
            int wp = _characterStats.GetAttribute(AttributeType.Composure).GetValue() + _characterStats.GetAttribute(AttributeType.Resolve).GetValue();
            _characterStats.GetTracker(TrackerType.Willpower).SetValue(wp);
            Willpower = _characterStats.GetTracker(TrackerType.Willpower).GetValue();
            Humanity = _characterStats.GetTracker(TrackerType.Humanity).GetValue();
            Hunger = _characterStats.GetTracker(TrackerType.Hunger).GetValue();
        }

        public void TakeDamage(int damage)
        {
            int newHealth = Health - damage;
            Debug.Log(owner.gameObject.name + " takes " + damage + " damage!" + "\nCurrent health: " + Health + "\nHealth after attack: " + newHealth);
            Health = newHealth < 0 ? 0 : newHealth;
            if (Health <= 0)
                Die();
        }

        public virtual void Die()
        {
            Debug.Log("Character died!");
        }

        public int GetCombatSpeed()
        {
            Attribute dexterity = _characterStats.GetAttribute(AttributeType.Dexterity);
            Skill athletics = _characterStats.GetSkill(SkillType.Athletics);
            if (dexterity.GetAttributeType() != AttributeType.Invalid && athletics.GetSkillType() != SkillType.Invalid)
                return (dexterity.GetValue() + athletics.GetValue());
            else
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
            return Health > 0;
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

        public Outcome GetSkillRoll(AttributeType attribute, SkillType skill)
        {
            Attribute a = _characterStats.GetAttribute(attribute);
            Skill s = _characterStats.GetSkill(skill);
            int normalDice = a.GetValue() + s.GetValue();
            int hungerDice = 0;
            if (owner.Data.CreatureType.Equals(CreatureType.Vampire))
            {
                hungerDice = Hunger;
                normalDice = hungerDice <= normalDice ? normalDice - hungerDice : 0;
            }

            return Roll.d10(normalDice, hungerDice);
        }

        public Outcome GetSkillRoll(AttributeType attribute, SkillType skill, int dc)
        {
            Attribute a = _characterStats.GetAttribute(attribute);
            Skill s = _characterStats.GetSkill(skill);
            int normalDice = a.GetValue() + s.GetValue();
            int hungerDice = 0;
            if (owner.Data.CreatureType.Equals(CreatureType.Vampire))
            {
                hungerDice = Hunger;
                normalDice = hungerDice <= normalDice ? normalDice - hungerDice : 0;
            }

            return Roll.d10(normalDice, hungerDice, dc);
        }

        public Outcome GetAttackRoll(int dc)
        {
            AttributeType weaponAttribute = GetWeaponAttribute();
            SkillType weaponSkill = GetWeaponSkill();
            return GetSkillRoll(weaponAttribute, weaponSkill, dc);
        }

        public List<Attribute> GetAttributes()
        {
            return _characterStats.GetAttributes();
        }

        internal HealthData GetHealthData()
        {
            HealthData.HealthDataBuilder builder = new HealthData.HealthDataBuilder(Stats.GetTracker(TrackerType.Health).GetValue());
            return builder.Create();
        }
    } 
}
