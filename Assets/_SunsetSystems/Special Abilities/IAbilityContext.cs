using SunsetSystems.Combat;
using SunsetSystems.Entities.Characters.Actions;

namespace SunsetSystems.Abilities
{
    public interface IAbilityContext
    {
        IAbilityUser User { get; }
        IActionPerformer ActionPerformer { get; }
        ICombatant UserCombatBehaviour { get; }
    }
}
