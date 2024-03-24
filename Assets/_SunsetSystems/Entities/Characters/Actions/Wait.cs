using SunsetSystems.Entities.Characters.Actions.Conditions;
using UnityEngine;

namespace SunsetSystems.Entities.Characters.Actions
{
    public class Wait : EntityAction
    {
        public Wait(float duration, IActionPerformer owner) : base(owner)
        {
            conditions.Add(new TimeElapsed(Time.time, duration));
        }

        public override void Begin()
        {
            
        }
    }
}
