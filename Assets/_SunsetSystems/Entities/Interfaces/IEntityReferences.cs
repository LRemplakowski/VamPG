using UnityEngine;

namespace SunsetSystems.Entities.Interfaces
{
    public interface IEntityReferences
    {
        Transform Transform { get; }
        GameObject GameObject { get; }

        T GetComponent<T>() where T : Component;
        T GetComponentInChildren<T>() where T : Component;
    }
}
