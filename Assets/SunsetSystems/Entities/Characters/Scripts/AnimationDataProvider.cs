using SunsetSystems.Inventory.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace SunsetSystems.Animation
{
    [RequireComponent(typeof(RigTransform))]
    public class AnimationDataProvider : MonoBehaviour
    {
        [field:SerializeField]
        public Vector3 PositionOffset { get; private set; }
        [field: SerializeField]
        public Vector3 RotationOffset { get; private set; }
        [field: SerializeField]
        public Transform RightHandIK { get; private set; }
        [field: SerializeField]
        public Transform LeftHandIK { get; private set; }
        [field: SerializeField]
        public Vector3 LeftHintLocalPosition { get; private set; }
        [field: SerializeField]
        public Vector3 RightHintLocalPosition { get; private set; }
        [field: SerializeField]
        public WeaponType AnimationType { get; private set; }
    }
}
