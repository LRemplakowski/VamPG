using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities.Cover
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
