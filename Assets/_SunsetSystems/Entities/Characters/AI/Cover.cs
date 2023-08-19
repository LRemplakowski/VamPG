using CleverCrow.Fluid.UniqueIds;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Entities
{
    [RequireComponent(typeof(UniqueId))]
    public class Cover : PersistentEntity
    {
        [SerializeField]
        private UniqueId uniqueID;
        public override string ID => uniqueID.Id;
        [SerializeField]
        private CoverQuality coverQuality;

        protected override void Awake()
        {
            base.Awake();
            uniqueID ??= GetComponent<UniqueId>();
        }

        public CoverQuality GetCoverQuality()
        {
            return coverQuality;
        }
    } 
}
