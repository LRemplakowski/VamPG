using SunsetSystems.Inventory.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace SunsetSystems.Animation
{
    [RequireComponent(typeof(RigTransform))]
    public class WeaponAnimationDataProvider : MonoBehaviour
    {
        [field: SerializeField]
        public Transform RightHandIK { get; private set; }
        [field: SerializeField]
        public Transform LeftHandIK { get; private set; }
        [field: SerializeField]
        public Vector3 LeftHintLocalPosition { get; private set; }
        [field: SerializeField]
        public Vector3 RightHintLocalPosition { get; private set; }
        [field: SerializeField]
        public WeaponAnimationType AnimationType { get; private set; }
    }

    public enum WeaponAnimationType
    {
        Brawl = 0, Pistol = 1, Rifle = 2, MeleeOneHanded = 4, MeleeTwoHanded = 5
    }
}
