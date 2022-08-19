using UnityEngine;

namespace Entities
{
    [RequireComponent(typeof(Collider))]
    public abstract class Entity : ExposableMonobehaviour
    {
        public static bool IsInteractable(GameObject entity, out IInteractable interactable)
        {
            interactable = entity.GetComponent<IInteractable>();
            return interactable != null;
        }
    }
}
