using Sirenix.OdinInspector;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace SunsetSystems.Entities.Characters.Actions.Conditions
{
    [System.Serializable]
    public class FaceTargetCondition : Condition
    {
        [SerializeField]
        private Vector3 faceDirection;
        [SerializeField]
        private Transform rotatingTransform;
        [SerializeField]
        private float marginOfError;

        public FaceTargetCondition(Transform rotatingTransform, Vector3 faceDirection, float marginOfError = 0.1f)
        {
            this.rotatingTransform = rotatingTransform;
            this.faceDirection = faceDirection.normalized;
            this.marginOfError = marginOfError;
        }

        public override bool IsMet()
        {
            return !(Vector3.Dot(rotatingTransform.forward, faceDirection) < 1f - marginOfError);
        }

        public override string ToString()
        {
            return $"Face target: Current forward({rotatingTransform.forward}), Expected forward({faceDirection}), Dot product({Vector3.Dot(rotatingTransform.forward, faceDirection)})";
        }
    }
}