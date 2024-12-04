using System.Collections.Generic;
using SunsetSystems.Abilities;
using UnityEngine;

namespace SunsetSystems.Combat
{
    public interface ICombatContext
    {
        GameObject GameObject { get; }
        Transform Transform { get; }
        Vector3 AimingOrigin { get; }
        bool IsInCover { get; }
        bool IsPlayerControlled { get; }
        bool IsUsingPrimaryWeapon { get; }
        bool IsSelectedWeaponUsingAmmo { get; }
        int SelectedWeaponDamageBonus { get; }
        int SelectedWeaponCurrentAmmo { get; }
        int SelectedWeaponMaxAmmo { get; }

        IAbilityUser AbilityUser { get; }

        IEnumerable<ICover> CurrentCoverSources { get; }

        int GetAttributeValue(AttributeType attribute);
        int GetSkillValue(SkillType skill);
    }
}
