using static UnityEngine.InputSystem.InputAction;

namespace SunsetSystems.Input
{
    public interface IGameplayInputHandler
    {
        void HandlePrimaryAction(CallbackContext context);

        void HandleSecondaryAction(CallbackContext context);

        void HandlePointerPosition(CallbackContext context);
    }
}
