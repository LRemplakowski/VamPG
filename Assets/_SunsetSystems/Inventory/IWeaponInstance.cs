using SunsetSystems.Animation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Equipment
{
    public interface IWeaponInstance
    {
        Transform MuzzleFlashOrigin { get; }
        Transform ProjectileOrigin { get; }
        WeaponAnimationDataProvider WeaponAnimationData { get; }
    }
}
