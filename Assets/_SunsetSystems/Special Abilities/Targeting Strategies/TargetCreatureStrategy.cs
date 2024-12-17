using System;
using SunsetSystems.Combat;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Inventory;
using UnityEngine;

namespace SunsetSystems.Abilities.Targeting
{
    public class TargetCreatureStrategy : IAbilityTargetingStrategy
    {
        private readonly IAbilityConfig _ability;

        private event Action ExecuteAbility;

        public TargetCreatureStrategy(IAbilityConfig ability)
        {
            _ability = ability;
        }

        public void ExecuteTargetSelect(ITargetingContext context)
        {
            //throw new System.NotImplementedException();
        }

        public void ExecuteClearTargetLock(ITargetingContext context)
        {
            //throw new System.NotImplementedException();
        }

        public void ExecutePointerPosition(ITargetingContext context)
        {
            Collider collider = context.GetLastRaycastCollider();
            LineRenderer targetingLineRenderer = context.GetTargetingLineRenderer();
            if (collider.gameObject.TryGetComponent(out ICreature targetCreature) is false)
            {
                context.TargetUpdateDelegate().Invoke(null);
                context.TargetingLineUpdateDelegate().Invoke(false);
                return;
            }
            ICombatant current = context.GetCurrentCombatant();
            ICombatant target = targetCreature.References.CombatBehaviour;
            if (target.GetContext().IsAlive)
            {
                context.TargetUpdateDelegate().Invoke(null);
                context.TargetingLineUpdateDelegate().Invoke(false);
                return;
            }
            context.TargetUpdateDelegate().Invoke(target.References.Targetable);
            var abilityRange = _ability.GetTargetingData(context.GetAbilityContext()).GetRangeData();
            if (IsTargetInRange(current, target, in abilityRange))
            {
                targetingLineRenderer.SetPosition(0, current.AimingOrigin);
                targetingLineRenderer.SetPosition(1, target.AimingOrigin);
                context.TargetingLineUpdateDelegate().Invoke(true);
                current.References.NavigationManager.FaceDirectionAfterMovementFinished(target.Transform.position);
            }
        }

        public void ExecuteTargetingBegin(ITargetingContext context)
        {
            context.TargetUpdateDelegate().Invoke(null);
            context.TargetingLineUpdateDelegate().Invoke(false);
        }

        public void ExecuteTargetingEnd(ITargetingContext context)
        {
            context.TargetUpdateDelegate().Invoke(null);
            context.TargetingLineUpdateDelegate().Invoke(false);
        }

        private static bool IsTargetInRange(ICombatant attacker, ICombatant target, in RangeData abilityRange)
        {
            Vector3 attackerPosition = attacker.Transform.position;
            Vector3 targetPosition = target.Transform.position;
            return Vector3.Distance(attackerPosition, targetPosition) <= abilityRange.MaxRange;
        }

        public void AddUseAbilityListener(Action listener)
        {
            ExecuteAbility += listener;
        }

        public void RemoveUseAbilityListener(Action listener)
        {
            ExecuteAbility -= listener;
        }
    }
}
