using System;
using UnityEngine;

namespace SunsetSystems.Entities
{
    public interface IHoverNameplateSource
    {
        public static Action<IHoverNameplateSource, bool> OnHoverStatusChange;

        GameObject NameplateSource { get; }
        string NameplateText { get; }
        Vector3 NameplateWorldPosition { get; }
    }
}
