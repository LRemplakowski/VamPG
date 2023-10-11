using Entities.Characters.Data;
using System.Collections.Generic;
using UnityEngine;
using SunsetSystems.Dice;
using SunsetSystems.Entities.Data;
using SunsetSystems.Spellbook;
using System;
using static SunsetSystems.Spellbook.DisciplinePower.EffectWrapper;
using SunsetSystems.Entities.Characters.Interfaces;

namespace SunsetSystems.Entities.Characters
{
    public class StatsManager : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        private Creature _owner;
        private Creature Owner
        {
            get
            {
                if (_owner == null)
                    _owner = GetComponent<Creature>();
                return _owner;
            }
        }

        [field: SerializeField]
        public StatsData Stats { get; private set; }

        public Tracker Health => Stats.Trackers.GetTracker(TrackerType.Health);
        public Tracker Willpower => Stats.Trackers.GetTracker(TrackerType.Willpower);
        public Tracker Hunger => Stats.Trackers.GetTracker(TrackerType.Hunger);
        public Tracker Humanity => Stats.Trackers.GetTracker(TrackerType.Humanity);

        public StatsManager Instance { get; protected set; }

        public event Action<Creature> OnCreatureDied;

        private void OnValidate()
        {
            _owner ??= GetComponent<Creature>();
        }

        public void Initialize(Creature owner)
        {
            this._owner = owner;
        }

        public void TakeDamage(int damage)
        {
            Health.SuperficialDamage += damage;
            if (Health.GetValue() <= 0)
                Die();
        }

        public bool TryUseBlood(int amount)
        {
            if (amount > Hunger.GetValue())
                return false;
            Hunger.SuperficialDamage += amount;
            return true;
        }

        public void RegainBlood(int amount)
        {
            if (amount > Hunger.SuperficialDamage)
                Hunger.SuperficialDamage = 0;
            else
                Hunger.SuperficialDamage -= amount;
        }

        public virtual void Die()
        {
            Health.SuperficialDamage = 10000;
            OnCreatureDied?.Invoke(Owner);
        }

        internal void Heal(int amount)
        {
            int currentDamage = Health.SuperficialDamage;
            currentDamage -= amount;
            currentDamage = currentDamage < 0 ? 0 : currentDamage;
            Health.SuperficialDamage = currentDamage;
        }

        public int GetCombatSpeed()
        {
            //return Stats.Attributes.GetAttribute(AttributeType.Speed).GetValue();
            return 5;
        }

        public int GetInitiative()
        {
            return Stats.Attributes.GetAttribute(AttributeType.Dexterity).GetValue();
        }

        public bool IsAlive()
        {
            return Health.GetValue() > 0;
        }

        public AttributeSkillPool GetDefensePool()
        {
            return new AttributeSkillPool(Stats.Attributes.GetAttribute(AttributeType.Dexterity), Stats.Skills.GetSkill(SkillType.Athletics));
        }

        public AttributeSkillPool GetAttackPool()
        {
            return new AttributeSkillPool(Stats.Attributes.GetAttribute(GetWeaponAttribute()), Stats.Skills.GetSkill(GetWeaponSkill()));
        }

        private AttributeType GetWeaponAttribute()
        {
            return AttributeType.Composure;
        }

        private SkillType GetWeaponSkill()
        {
            return SkillType.Firearms;
        }

        public DisciplinePower GetDisciplinePower(string powerID)
        {
            foreach (Discipline discipline in Stats.Disciplines.GetDisciplines())
            {
                DisciplinePower power = discipline.GetKnownPowers().Find(p => p.ID.Equals(powerID));
                if (power != null)
                    return power;
            }
            return null;
        }

        public void CopyFromTemplate(ICreatureTemplate template)
        {
            Stats = new(template.StatsData);
        }

        public Outcome GetSkillRoll(AttributeType attribute, SkillType skill, bool useHunger = false)
        {
            CreatureAttribute a = Stats.Attributes.GetAttribute(attribute);
            Skill s = Stats.Skills.GetSkill(skill);
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
            CreatureAttribute a = Stats.Attributes.GetAttribute(attribute);
            Skill s = Stats.Skills.GetSkill(skill);
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
            return Stats.Attributes.GetAttributeList();
        }

        public HealthData GetHealthData()
        {
            HealthData.HealthDataBuilder builder = new(Stats.Trackers.GetTracker(TrackerType.Health).GetValue());
            return builder.Create();
        }

        public void ApplyEffect(AttributeEffect effect)
        {
            Stats.Attributes.GetAttribute(effect.AffectedProperty).AddModifier(new(effect.ModifierValue, effect.ModifierType, effect.GetHashCode().ToString()));
        }

        public void ApplyEffect(SkillEffect effect)
        {
            throw new NotImplementedException();
        }

        public void ApplyEffect(DisciplineEffect effect)
        {
            throw new NotImplementedException();
        }
    }
}
