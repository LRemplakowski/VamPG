using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace SunsetSystems.Entities.Characters.Actions.Conditions
{
    public class FaceTargetCondition : Condition
    {
        private Task faceTargetTask;

        public FaceTargetCondition(Task faceTargetTask)
        {
            this.faceTargetTask = faceTargetTask;
        }

        public override bool IsMet()
        {
            return faceTargetTask.IsCompleted;
        }

        public override string ToString()
        {
            return "";
        }
    }
}