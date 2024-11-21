using SunsetSystems.Entities.Characters;
using SunsetSystems.Entities.Interfaces;
using SunsetSystems.Entities;
using SunsetSystems.Inventory.Data;
using SunsetSystems.Spellbook;
using System;
using UnityEngine;
using SunsetSystems.Inventory;

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

        public static AttackResult CalculateAttackResult(ICombatant attacker, ICombatant defender)
        {
            return CalculateAttackResult(attacker, defender, new());
        }

        public static AttackResult CalculateAttackResult(ICombatant attacker, ICombatant defender, AttackModifier attackModifier)
        {
            int damage = 0;
            int damageReduction = 0;
            int adjustedDamage = 0;
            double critChance = 0;
            double critRoll = 0;

            float heightDifference = attacker.References.Transform.position.y - defender.References.Transform.position.y;
            if (heightDifference > 2f && attacker.References.GetCachedComponentInChildren<SpellbookManager>().GetIsPowerKnown(PassivePowersHelper.Instance.HeightAttackAndDamageBonus))
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
                damageReduction = CalculateDefenderDamageReduction(defender, attacker.WeaponManager.GetSelectedWeapon().WeaponType) + attackModifier.DamageReductionMod;
                adjustedDamage = (damage > damageReduction ? damage - damageReduction : 1) + attackModifier.AdjustedDamageMod;
            }
            if (attacker.IsPlayerControlled)
            {
                Debug.LogError($"ADDED 10 TO PLAYER DAMAGE! THIS IS TEMPORARY, REMOVE IT!");
                adjustedDamage += 10;
            }

            return new(attackerHitChance, defenderDodgeChance, hitRoll, critChance, critRoll, damage, damageReduction, adjustedDamage, hit, crit);
        }

        private static double CalculateCritChance(ICombatant attacker)
        {
            double result = BASE_CRIT_CHANCE;
            result += attacker.GetAttributeValue(AttributeType.Wits) * 0.01d;
            return result;
        }

        private static int CalculateDefenderDamageReduction(ICombatant defender, Inventory.AbilityRange attackType)
        {
            int damageReduction = 0;
            switch (attackType)
            {
                case Inventory.AbilityRange.Melee:
                    damageReduction += defender.GetAttributeValue(AttributeType.Stamina);
                    break;
                case Inventory.AbilityRange.Ranged:
                    damageReduction += defender.GetAttributeValue(AttributeType.Dexterity);
                    break;
            }
            return damageReduction;
        }

        private static int CalculateAttackDamage(ICombatant attacker, ICombatant defender)
        {
            int damage = 0;
            IWeapon selectedWeapon = attacker.WeaponManager.GetSelectedWeapon();
            float weaponDamageMod = attacker.WeaponManager.GetPrimaryWeapon().Equals(selectedWeapon) ? 1f : 0.6f;
            switch (selectedWeapon.WeaponType)
            {
                case Inventory.AbilityRange.Melee:
                    damage = Mathf.RoundToInt((attacker.GetAttributeValue(AttributeType.Strength) + selectedWeapon.GetDamageData().DamageModifier) * weaponDamageMod);
                    break;
                case Inventory.AbilityRange.Ranged:
                    damage = Mathf.RoundToInt((attacker.GetAttributeValue(AttributeType.Composure) + selectedWeapon.GetDamageData().DamageModifier) * weaponDamageMod);
                    break;
            }
            damage = Mathf.Clamp(damage, 1, int.MaxValue);
            return damage;
        }

        private static double CalculateHitChance(ICombatant attacker, ICombatant defender)
        {
            double attributeModifier = attacker.GetAttributeValue(AttributeType.Wits);
            double result = 0d;
            if (attacker.WeaponManager.GetSelectedWeapon().GetRangeData().ShortRange >= Vector3.Distance(attacker.References.Transform.position, defender.References.Transform.position))
                result -= SHORT_RANGE_HIT_PENALTY;
            result += BASE_HIT_CHANCE + (attributeModifier * 0.01d);
            return result;
        }

        private static double CalculateDodgeChance(ICombatant defender, ICombatant attacker)
        {
            double result = BASE_DODGE_CHANCE;
            bool hasCover = CoverDetector.FiringLineObstructedByCover(attacker, defender, out ICover coverSource);
            if (hasCover)
            {
                int defenderDexterity = defender.GetAttributeValue(AttributeType.Dexterity);
                switch (coverSource.Quality)
                {
                    case CoverQuality.Half:
                        result += COVER_MODIFIER_LOW + (defenderDexterity * 0.2d);
                        break;
                    case CoverQuality.Full:
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
