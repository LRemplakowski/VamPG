using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace SunsetSystems.Entities.Characters.Actions.Conditions
{
    public class FaceTargetCondition : Condition
    {
        private Vector3 faceDirection;
        private Transform rotatingTransform;

        public FaceTargetCondition(Transform rotatingTransform, Vector3 faceDirection)
        {
            this.rotatingTransform = rotatingTransform;
            this.faceDirection = faceDirection.normalized;
        }

        public override bool IsMet()
        {
            return Vector3.Dot(rotatingTransform.forward, faceDirection) >= 1f;
        }

        public override string ToString()
        {
            return $"Face target: Current forward({rotatingTransform.forward}), Expected forward({faceDirection}), Dot product({Vector3.Dot(rotatingTransform.forward, faceDirection)})";
        }
    }
}