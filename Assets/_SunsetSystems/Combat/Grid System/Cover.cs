using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Combat
{
    public class Cover : MonoBehaviour, ICover
    {
        [field: SerializeField]
        public CoverQuality Quality { get; private set; }

        public Vector3 WorldPosition => transform.position;
    }
}
