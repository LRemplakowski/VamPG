using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace SunsetSystems.ActionSystem
{
    [Serializable]
    public class WaitForFlag : Condition
    {
        [SerializeField]
        private FlagWrapper flag;

        public WaitForFlag(FlagWrapper flag)
        {
            this.flag = flag;
        }

        public override bool IsMet()
        {
            return flag.Value;
        }
    }

    [Serializable]
    public class FlagWrapper
    {
        [field: SerializeField]
        public bool Value { get; set; }
    }
}
