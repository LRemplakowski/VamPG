using UnityEngine;

namespace SunsetSystems.Entities
{
    public interface INameplateReciever
    {
        string NameplateText { get; }

        Vector3 NameplateWorldPosition { get; }
    }
}
