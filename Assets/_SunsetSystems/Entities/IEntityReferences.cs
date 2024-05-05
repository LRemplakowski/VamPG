using UnityEngine;

namespace SunsetSystems.Entities.Interfaces
{
    public interface IEntityReferences
    {
        Transform Transform { get; }
        GameObject GameObject { get; }

        T GetCachedComponent<T>();
        T GetCachedComponentInChildren<T>();
    }
}
