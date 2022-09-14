using Entities;

namespace SunsetSystems.Inventory
{
    public interface IScriptableItem
    {
        void InvokeBehaviour(Entity user, Entity target);
    }
}
