using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace SunsetSystems.WorldMap
{
    public class WorldMapPlayerToken : SerializedMonoBehaviour
    {
        [SerializeField]
        private NavMeshAgent _navMeshAgent;

        public void MoveToToken(IWorldMapToken token, bool warp = false)
        {
            Vector3 targetPosition = NavMeshPointFromWorldPosition(token.GetPlayerTokenDestination());
            if (warp)
                _navMeshAgent.Warp(targetPosition);
            else
                _navMeshAgent.SetDestination(targetPosition);
        }

        private Vector3 NavMeshPointFromWorldPosition(Vector3 worldPosition)
        {
            if (NavMesh.SamplePosition(worldPosition, out var hit, 1f, NavMesh.AllAreas))
            {
                return hit.position;
            }
            return worldPosition;
        }
    }
}
