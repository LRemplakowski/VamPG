using SunsetSystems.Entities.Characters.Actions;

namespace SunsetSystems.Entities.Interactable
{
    public interface IInteractionHandler
    {
        bool HandleInteraction(IActionPerformer interactee);
    }
}
