using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Entities.Cover
{
    public class Cover : Entity
    {
        [SerializeField]
        private CoverQuality coverQuality;

        public CoverQuality GetCoverQuality()
        {
            return coverQuality;
        }
    } 
}
