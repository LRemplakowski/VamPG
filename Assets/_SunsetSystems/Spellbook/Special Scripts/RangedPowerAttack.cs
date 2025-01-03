using SunsetSystems.Combat;
using SunsetSystems.Entities.Characters;
using SunsetSystems.ActionSystem;
using SunsetSystems.Entities.Interfaces;
using System;
using UnityEngine;

namespace SunsetSystems.Abilities
{
    [CreateAssetMenu(fileName = "RangedPowerAttack", menuName = "Scriptable Powers/Ranged Power Attack")]
    public class RangedPowerAttack : DisciplineScript
    {
        [SerializeField]
        private float _composureMultiplier = 0.75f;

        public override void Activate(ICombatant target, ICombatant caster)
        {
            //AttackModifier modifier = new();
            //int casterComposure = caster.References.StatsManager.Stats.Attributes.GetAttribute(AttributeType.Composure).GetValue();
            //modifier.DamageMod = Mathf.RoundToInt(casterComposure * _composureMultiplier);
            //modifier.ForceCritical = true;
            //modifier.HitChanceMod = -0.3d;
            //Attack attack = new(target, caster, modifier);
            //caster.PerformAction(attack);
            throw new NotImplementedException();
        }
    }
}
