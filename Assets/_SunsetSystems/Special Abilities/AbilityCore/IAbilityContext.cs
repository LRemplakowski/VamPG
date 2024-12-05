using SunsetSystems.Combat;
using SunsetSystems.ActionSystem;
using SunsetSystems.Combat.Grid;

namespace SunsetSystems.Abilities
{
    public interface IAbilityContext
    {
        IActionPerformer SourceActionPerformer { get; }
        ICombatant SourceCombatBehaviour { get; }
        ITargetable TargetObject { get; }
        IGridCell TargetPosition { get; }
        GridManager GridManager { get; }
    }
}