using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Combat
{
    public interface ICombatContext
    {
        GameObject GameObject { get; }
        Transform Transform { get; }
        bool IsInCover { get; }
        bool IsPlayerControlled { get; }
        bool IsUsingPrimaryWeapon { get; }
        Vector3 AimingOrigin { get; }
        int CurrentWeaponDamageBonus { get; }

        IEnumerable<ICover> CurrentCoverSources { get; }

        int GetAttributeValue(AttributeType attribute);
        int GetSkillValue(SkillType skill);
    }
}
