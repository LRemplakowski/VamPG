using System;
using SunsetSystems.Combat;
using SunsetSystems.Combat.Grid;
using UnityEngine;

namespace SunsetSystems.Abilities
{
    public interface ITargetingContext
    {
        Action<ITargetable> UpdateTargetDelegate();
        Action<bool> UpdateTargetingLineVisibilityDelegate();

        IAbilityConfig GetSelectedAbility();
        Collider GetLastRaycastCollider();
        LineRenderer GetTargetingLineRenderer();
        IAbilityContext GetAbilityContext();
        ICombatant GetCurrentCombatant();
        GridManager GetCurrentGrid();
    }
}
