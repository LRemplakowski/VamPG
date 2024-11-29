using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SunsetSystems.Abilities
{
    [CreateAssetMenu(fileName = "New Weapon Ability", menuName = "Sunset Abilities/Weapon Ability")]
    public class WeaponAbility : BaseAbility
    {
        [BoxGroup("Weapon Ability")]
        [SerializeField]
        private int _numberOfAttacks = 1;
        [BoxGroup("Weapon Ability")]
        [SerializeField]
        private int _ammoUsed = 0;
    }
}
