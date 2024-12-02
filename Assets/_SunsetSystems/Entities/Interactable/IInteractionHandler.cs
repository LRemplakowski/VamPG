using SunsetSystems.ActionSystem;

namespace SunsetSystems.Entities.Interactable
{
    public interface IInteractionHandler
    {
        bool HandleInteraction(IActionPerformer interactee);
    }
}
