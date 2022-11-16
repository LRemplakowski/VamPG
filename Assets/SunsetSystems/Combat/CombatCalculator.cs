using SunsetSystems.Entities.Characters;
using SunsetSystems.Entities.Cover;
using SunsetSystems.Inventory.Data;
using UnityEngine;

namespace SunsetSystems.Combat
{
    public static class CombatCalculator
    {
        public const double BASE_HIT_CHANCE = 0.9d;
        public const double SHORT_RANGE_HIT_PENALTY = 0.5d;
        public const double BASE_DODGE_CHANCE = 0.05d;
        public const double COVER_MODIFIER_LOW = 0.2d;
        public const double COVER_MODIFIER_HIGH = 0.5d;

        private static readonly System.Random _random = new();

        public static AttackResult CalculateAttackResult(Creature attacker, Creature defender)
        {
            int damage = 0;
            int damageReduction = 0;
            int adjustedDamage = 0;
            double attackerHitChance = CalculateHitChance(attacker);
            double defenderDodgeChance = CalculateDodgeChance(defender, attacker);
            bool hit = false;
            hit |= attackerHitChance - defenderDodgeChance >= 1d;
            double hitRoll = _random.NextDouble();
            hit |= hitRoll < attackerHitChance - defenderDodgeChance;
            if (hit)
            {
                damage = CalculateAttackDamage(attacker);
                damageReduction = CalculateDefenderDamageReduction(defender, attacker.Data.Equipment.GetSelectedWeapon().WeaponType);
                adjustedDamage = damage > damageReduction ? damage - damageReduction : 1;
            }
            return new(attackerHitChance, defenderDodgeChance, hitRoll, damage, damageReduction, adjustedDamage, hit);
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

        private static int CalculateAttackDamage(Creature attacker)
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

        private static double CalculateHitChance(Creature attacker)
        {
            double attributeModifier = attacker.Data.Stats.Attributes.GetAttribute(attacker.Data.Equipment.GetSelectedWeapon().AssociatedAttribute).GetValue();
            double result = BASE_HIT_CHANCE + (attributeModifier * 0.01d);
            return result;
        }

        private static double CalculateDodgeChance(Creature defender, Creature attacker)
        {
            double result = BASE_DODGE_CHANCE;
            bool hasCover = CoverDetector.FiringLineObstructedByCover(attacker, defender, out Cover coverSource);
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

    public struct AttackResult
    {
        public readonly double AttackerHitChance, DefenderDodgeChance, HitRoll;
        public readonly int Damage, DamageReduction, AdjustedDamage;
        public readonly bool Successful;

        public AttackResult(double attackerHitChance, double defenderDodgeChance, double hitRoll, int damage, int damageReduction, int adjustedDamage, bool successful)
        {
            AttackerHitChance = attackerHitChance;
            DefenderDodgeChance = defenderDodgeChance;
            HitRoll = hitRoll;
            Damage = damage;
            DamageReduction = damageReduction;
            AdjustedDamage = adjustedDamage;
            Successful = successful;
        }
    }
}
