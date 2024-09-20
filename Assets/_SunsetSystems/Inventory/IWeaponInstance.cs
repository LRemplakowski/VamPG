using SunsetSystems.Animation;
using UnityEngine;

namespace SunsetSystems.Equipment
{
    public interface IWeaponInstance
    {
        GameObject GameObject { get; }
        WeaponAnimationDataProvider WeaponAnimationData { get; }

        void PlayFireWeaponFX();
        void PlayReloadSFX();
    }
}
