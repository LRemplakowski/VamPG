using UnityEngine;

namespace SunsetSystems.Entities.Interfaces
{
    public interface IEntityReferences
    {
        Transform Transform { get; }
        GameObject GameObject { get; }

        T GetComponent<T>();
        T GetComponentInChildren<T>();
    }
}
