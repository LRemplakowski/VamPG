using System.Collections;
using System.Collections.Generic;
using SunsetSystems.Inventory;
using UnityEngine;

namespace SunsetSystems.Combat
{
    public interface IAttackContext
    {
        int GetAttackDamage();
        int GetDamageReduction();
        int GetAttributeValue(AttackParticipant entity, AttributeType attributeType);
        Vector3 GetPosition(AttackParticipant entity);
        Vector3 GetAimingPosition(AttackParticipant entity);
        bool IsInCover(AttackParticipant entity);
        bool IsPlayerControlled(AttackParticipant entity);
        IEnumerable<ICover> GetCoverSources(AttackParticipant entity);
        AttackModifier GetBaseAttackModifier();
        AttackModifier GetHeightAttackModifier();
        RangeData GetAttackRangeData();
        AttackType GetAttackType();
    }

    public enum AttackParticipant
    {
        Attacker, Target
    }

    public enum AttackType
    {
        WeaponMelee, WeaponRanged, MagicMelee, MagicRanged, WeaponAOE, MagicAOE
    }
}
