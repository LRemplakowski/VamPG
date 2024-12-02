using System.Collections;
using System.Collections.Generic;
using SunsetSystems.Localization;
using UnityEngine;

namespace SunsetSystems.Combat
{
    public interface ICombatContext
    {
        Transform Transform { get; }
        bool IsInCover { get; }
        Vector3 AimingOrigin { get; }
        bool IsPlayerControlled { get; }

        IEnumerable<ICover> CurrentCoverSources { get; }

        int GetAttributeValue(AttributeType attribute);
    }
}
