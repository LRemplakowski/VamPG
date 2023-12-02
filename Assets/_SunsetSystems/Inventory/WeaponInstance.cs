using Sirenix.OdinInspector;
using SunsetSystems.Animation;
using SunsetSystems.Audio;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.VFX;

namespace SunsetSystems.Equipment
{
    public class WeaponInstance : SerializedMonoBehaviour, IWeaponInstance
    {
        [Title("Config")]
        [SerializeField, Min(0)]
        private float bulletVelocity = 3f;
        [SerializeField, Min(0)]
        private float bulletLifetime = 5f;
        [Title("References")]
        [SerializeField]
        private Transform projectileOrigin;
        [SerializeField]
        private VisualEffect muzzleFlash;
        [SerializeField]
        private Rigidbody bulletPrefab;
        [SerializeField]
        private AudioClip shotSFX;
        [field: SerializeField]
        public WeaponAnimationDataProvider WeaponAnimationData { get; private set; }

        public GameObject GameObject => this.gameObject;

        [Button]
        public void FireWeapon()
        {
            if (muzzleFlash != null)
                muzzleFlash.Play();
            if (bulletPrefab)
            {
                Rigidbody bulletInstance = Instantiate(bulletPrefab, projectileOrigin.position, Quaternion.identity);
                bulletInstance.AddForce(projectileOrigin.forward * bulletVelocity);
                Destroy(bulletInstance.gameObject, bulletLifetime);
            }
            if (shotSFX != null)
                AudioManager.Instance.PlaySFXOneShot(shotSFX);
        }
    }
}
