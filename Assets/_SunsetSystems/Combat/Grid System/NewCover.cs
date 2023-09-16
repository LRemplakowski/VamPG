using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Combat
{
    public class NewCover : MonoBehaviour, ICover
    {
        [field: SerializeField]
        public NewCoverQuality Quality { get; private set; }
    }
}
