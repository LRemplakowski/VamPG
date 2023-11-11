using Sirenix.OdinInspector;
using SunsetSystems.Animation;
using SunsetSystems.Audio;
using UnityEngine;
using UnityEngine.VFX;

namespace SunsetSystems.Equipment
{
    public class WeaponInstance : SerializedMonoBehaviour, IWeaponInstance
    {
        [Title("References")]
        [SerializeField]
        private Transform projectileOrigin;
        [SerializeField]
        private VisualEffect muzzleFlash;
        [SerializeField]
        private AudioClip shotSFX;
        [field: SerializeField]
        public WeaponAnimationDataProvider WeaponAnimationData { get; private set; }

        public GameObject GameObject => this.gameObject;

        [Button]
        public void FireWeapon()
        {
            muzzleFlash.Play();
            AudioManager.Instance.PlaySFXOneShot(shotSFX);
        }
    }
}
