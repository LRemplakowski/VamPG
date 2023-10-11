using Sirenix.OdinInspector;
using SunsetSystems.Animation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Equipment
{
    public class WeaponInstance : SerializedMonoBehaviour, IWeaponInstance
    {
        [field: SerializeField]
        public Transform MuzzleFlashOrigin { get; private set; }
        [field: SerializeField]
        public Transform ProjectileOrigin { get; private set; }
        [field: SerializeField]
        public WeaponAnimationDataProvider WeaponAnimationData { get; private set; }
    }
}
