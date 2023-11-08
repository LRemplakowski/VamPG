using Sirenix.OdinInspector;
using SunsetSystems.Entities.Characters.Actions;
using SunsetSystems.Entities.Characters.Actions.Conditions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Entities.Characters.Actions
{
    public class FaceTarget : EntityAction
    {
        [ShowInInspector]
        private readonly Transform ownerTransform;
        [ShowInInspector]
        private readonly float rotationSpeed;
        [ShowInInspector]
        private Vector3 lookDirection;
        private IEnumerator lookAtRoutine;

        public FaceTarget(IActionPerformer owner, Transform lookAtTarget, float rotationSpeed = 90f) : this(owner, (lookAtTarget.position - owner.Transform.position).normalized, rotationSpeed)
        {

        }

        public FaceTarget(IActionPerformer owner, Vector3 lookDirection, float rotationSpeed = 90f) : base(owner)
        {
            ownerTransform = owner.Transform;
            this.lookDirection = lookDirection;
            this.rotationSpeed = rotationSpeed;
            conditions.Add(new FaceTargetCondition(ownerTransform, lookDirection));
        }

        public override void Abort()
        {
            base.Abort();
            if (lookAtRoutine != null)
                (Owner as MonoBehaviour).StopCoroutine(lookAtRoutine);
        }

        public override void Begin()
        {
            lookAtRoutine = LookAtTargetOverTime();
            (Owner as MonoBehaviour).StartCoroutine(lookAtRoutine);
        }

        private IEnumerator LookAtTargetOverTime()
        {
            float dotProduct = Vector3.Dot(ownerTransform.forward, lookDirection);
            Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
            while (dotProduct < 1f)
            {
                ownerTransform.rotation = Quaternion.RotateTowards(ownerTransform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
                yield return null;
            }
            lookAtRoutine = null;
        }
    }
}
