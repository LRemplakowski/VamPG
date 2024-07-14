using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace SunsetSystems.Entities.Characters.Navigation
{
    public class NavigationManager : MonoBehaviour, INavigationManager
    {
        [Title("Config")]
        [SerializeField, Range(.1f, 10f)]
        private float _faceTargetTime = .5f;
        [Title("References")]
        [SerializeField, Required]
        private NavMeshAgent _navMeshAgent;

        private Coroutine _faceTargetCoroutine;

        public Vector3 Position => _navMeshAgent.transform.position;
        public bool FinishedCurrentPath
        {
            get
            {
                if (_navMeshAgent.pathPending)
                    return false;
                return _navMeshAgent.hasPath && GetAgentFinishedMovement();
            }
        }

        public bool IsMoving
        {
            get
            {
                if (_navMeshAgent.pathPending)
                    return false;
                return _navMeshAgent.hasPath && CurrentSpeed > 0.1f;
            }
        }

        public float CurrentSpeed => _navMeshAgent.velocity.magnitude;
        public float MaxSpeed => _navMeshAgent.speed;

        public bool Warp(Vector3 position) => _navMeshAgent.Warp(position);
        public bool CalculatePath(Vector3 targetPosition, NavMeshPath path) => _navMeshAgent.CalculatePath(targetPosition, path);

        private bool GetAgentFinishedMovement()
        {
            return _navMeshAgent.pathStatus switch
            {
                NavMeshPathStatus.PathComplete => _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance,
                NavMeshPathStatus.PathPartial => _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance,
                NavMeshPathStatus.PathInvalid => true,
                _ => false,
            };
        }

        public void FaceDirectionAfterMovementFinished(Vector3 point)
        {
            if (_faceTargetCoroutine != null)
                StopCoroutine(_faceTargetCoroutine);
            StartCoroutine(FaceTargetInTime(_faceTargetTime, point));
        }

        private IEnumerator FaceTargetInTime(float time, Vector3 targetPosition)
        {
            yield return new WaitUntil(() => IsMoving is false);
            Vector3 lookPosition = targetPosition - transform.position;
            lookPosition.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(lookPosition);
            Quaternion startRotation = transform.rotation;
            float slerp = 0f;
            while (slerp < time)
            {
                if (IsMoving)
                    yield break;
                slerp += Time.deltaTime;
                transform.rotation = Quaternion.Slerp(startRotation, targetRotation, slerp / time);
                yield return null;
            }
            transform.rotation = targetRotation;
        }

        public bool SetNavigationTarget(Vector3 target)
        {
            return _navMeshAgent.SetDestination(target);
        }

        public void StopMovement()
        {
            _navMeshAgent.ResetPath();
        }
    }
}
