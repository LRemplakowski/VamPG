using SunsetSystems.Entities.Characters;
using SunsetSystems.Entities.Cover;
using SunsetSystems.Inventory.Data;
using System;
using UnityEngine;
using Zenject;

namespace SunsetSystems.Combat
{
    public static class CombatCalculator
    {
        public const double BASE_HIT_CHANCE = 0.9d;
        public const double SHORT_RANGE_HIT_PENALTY = 0.5d;
        public const double BASE_CRIT_CHANCE = 0.2d;
        public const double BASE_DODGE_CHANCE = 0.05d;
        public const double COVER_MODIFIER_LOW = 0.2d;
        public const double COVER_MODIFIER_HIGH = 0.5d;

        private static readonly System.Random _random = new();

        [Inject]
        private static ICoverDetector _coverDetector;

        public static AttackResult CalculateAttackResult(Creature attacker, Creature defender)
        {
            return CalculateAttackResult(attacker, defender, new());
        }

        public static AttackResult CalculateAttackResult(Creature attacker, Creature defender, AttackModifier attackModifier)
        {
            int damage = 0;
            int damageReduction = 0;
            int adjustedDamage = 0;
            double critChance = 0;
            double critRoll = 0;

            float heightDifference = attacker.transform.position.y - defender.transform.position.y;
            if (heightDifference > attacker.Agent.height && attacker.SpellbookManager.GetIsPowerKnown(null))
            {
                attackModifier.HitChanceMod += .1d;
                attackModifier.DamageMod += 2;
            }

            double attackerHitChance = CalculateHitChance(attacker, defender) + attackModifier.HitChanceMod;
            double defenderDodgeChance = CalculateDodgeChance(defender, attacker) + attackModifier.DodgeChanceMod;
            bool hit = false;
            bool crit = false;
            hit |= attackerHitChance - defenderDodgeChance >= 1d;
            double hitRoll = _random.NextDouble() + attackModifier.HitRollMod;
            hit |= hitRoll < attackerHitChance - defenderDodgeChance;
            hit |= attackModifier.SuccessMod;
            if (hit)
            {
                critChance = CalculateCritChance(attacker) + attackModifier.CritChanceMod;
                damage = CalculateAttackDamage(attacker, defender) + attackModifier.DamageMod;
                critRoll = _random.NextDouble() + attackModifier.CritRollMod;
                crit |= critRoll < critChance;
                crit |= attackModifier.CriticalMod;
                if (crit)
                    damage = Mathf.RoundToInt(damage * 1.5f);
                damageReduction = CalculateDefenderDamageReduction(defender, attacker.Data.Equipment.GetSelectedWeapon().WeaponType) + attackModifier.DamageReductionMod;
                adjustedDamage = (damage > damageReduction ? damage - damageReduction : 1) + attackModifier.AdjustedDamageMod;
            }
            return new(attackerHitChance, defenderDodgeChance, hitRoll, critChance, critRoll, damage, damageReduction, adjustedDamage, hit, crit);
        }

        private static double CalculateCritChance(Creature attacker)
        {
            double result = BASE_CRIT_CHANCE;
            result += attacker.Data.Stats.Attributes.GetAttribute(AttributeType.Wits).GetValue() * 0.01d;
            return result;
        }

        private static int CalculateDefenderDamageReduction(Creature defender, WeaponType attackType)
        {
            int damageReduction = 0;
            switch (attackType)
            {
                case WeaponType.Melee:
                    damageReduction += defender.Data.Stats.Attributes.GetAttribute(AttributeType.Stamina).GetValue();
                    break;
                case WeaponType.Ranged:
                    damageReduction += defender.Data.Stats.Attributes.GetAttribute(AttributeType.Dexterity).GetValue();
                    break;
            }
            return damageReduction;
        }

        private static int CalculateAttackDamage(Creature attacker, Creature defender)
        {
            int damage = 0;
            Weapon selectedWeapon = attacker.Data.Equipment.GetSelectedWeapon();
            float weaponDamageMod = attacker.Data.Equipment.GetPrimaryWeapon() == selectedWeapon ? 1f : 0.6f;
            switch (selectedWeapon.WeaponType)
            {
                case WeaponType.Melee:
                    damage = Mathf.RoundToInt(attacker.Data.Stats.Attributes.GetAttribute(AttributeType.Strength).GetValue() * weaponDamageMod);
                    break;
                case WeaponType.Ranged:
                    damage = Mathf.RoundToInt(attacker.Data.Stats.Attributes.GetAttribute(AttributeType.Composure).GetValue() * weaponDamageMod);
                    break;
            }
            damage = Mathf.Clamp(damage, 1, int.MaxValue);
            return damage;
        }

        private static double CalculateHitChance(Creature attacker, Creature defender)
        {
            double attributeModifier = attacker.Data.Stats.Attributes.GetAttribute(AttributeType.Wits).GetValue();
            double result = 0d;
            if (attacker.Data.Equipment.GetSelectedWeapon().GetRangeData().shortRange >= Vector3.Distance(attacker.transform.position, defender.transform.position))
                result -= SHORT_RANGE_HIT_PENALTY;
            result += BASE_HIT_CHANCE + (attributeModifier * 0.01d);
            return result;
        }

        private static double CalculateDodgeChance(Creature defender, Creature attacker)
        {
            double result = BASE_DODGE_CHANCE;
            bool hasCover = _coverDetector.FiringLineObstructedByCover(attacker, defender, out Cover coverSource);
            if (hasCover)
            {
                int defenderDexterity = defender.Data.Stats.Attributes.GetAttribute(AttributeType.Dexterity).GetValue();
                switch (coverSource.GetCoverQuality())
                {
                    case CoverQuality.Half:
                        result += COVER_MODIFIER_LOW + (defenderDexterity * 0.2d);
                        break;
                    case CoverQuality.High:
                        result += COVER_MODIFIER_HIGH + (defenderDexterity * 0.5d);
                        break;
                }
            }
            return result;
        }
    }

    [Serializable]
    public struct AttackModifier
    {
        public double HitChanceMod, DodgeChanceMod, HitRollMod;
        public double CritChanceMod, CritRollMod;
        public int DamageMod, DamageReductionMod, AdjustedDamageMod;
        public bool SuccessMod, CriticalMod;
    }

    public struct AttackResult
    {
        public readonly double AttackerHitChance, DefenderDodgeChance, HitRoll;
        public readonly double CritChance, CritRoll;
        public readonly int Damage, DamageReduction, AdjustedDamage;
        public readonly bool Successful, Critical;

        public AttackResult(double attackerHitChance, double defenderDodgeChance, double hitRoll, double critChance, double critRoll, int damage, int damageReduction, int adjustedDamage, bool successful, bool critical)
        {
            AttackerHitChance = attackerHitChance;
            DefenderDodgeChance = defenderDodgeChance;
            HitRoll = hitRoll;
            CritChance = critChance;
            CritRoll = critRoll;
            Damage = damage;
            DamageReduction = damageReduction;
            AdjustedDamage = adjustedDamage;
            Successful = successful;
            Critical = critical;
        }
    }
}
