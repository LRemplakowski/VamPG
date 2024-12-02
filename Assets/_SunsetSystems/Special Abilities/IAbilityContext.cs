using SunsetSystems.Combat;
using SunsetSystems.ActionSystem;

namespace SunsetSystems.Abilities
{
    public interface IAbilityContext
    {
        IAbilityUser User { get; }
        IActionPerformer ActionPerformer { get; }
        ICombatant UserCombatBehaviour { get; }
    }
}
