using SunsetSystems.Combat;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Entities.Characters.Actions;
using UnityEngine;

namespace SunsetSystems.Spellbook
{
    [CreateAssetMenu(fileName = "RangedPowerAttack", menuName = "Scriptable Powers/Ranged Power Attack")]
    public class RangedPowerAttack : DisciplineScript
    {
        [SerializeField]
        private float _composureMultiplier = 0.75f;

        public override void Activate(Creature target, Creature caster)
        {
            AttackModifier modifier = new();
            int casterComposure = caster.References.StatsManager.Stats.Attributes.GetAttribute(AttributeType.Composure).GetValue();
            modifier.DamageMod = Mathf.RoundToInt(casterComposure * _composureMultiplier);
            modifier.CriticalMod = true;
            modifier.HitChanceMod = -0.3d;
            Attack attack = new(target, caster, modifier);
            caster.PerformAction(attack);
        }
    }
}
