using SunsetSystems.Animation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace SunsetSystems.Equipment
{
    public interface IWeaponInstance
    {
        GameObject GameObject { get; }

        WeaponAnimationDataProvider WeaponAnimationData { get; }

        void FireWeapon();
    }
}
