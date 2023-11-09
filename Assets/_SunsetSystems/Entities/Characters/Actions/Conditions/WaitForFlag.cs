using Sirenix.OdinInspector;
using System;

namespace SunsetSystems.Entities.Characters.Actions.Conditions
{
    [Serializable]
    public class WaitForFlag : Condition
    {
        [ShowInInspector]
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
        [field: ShowInInspector]
        public bool Value { get; set; }
    }
}
