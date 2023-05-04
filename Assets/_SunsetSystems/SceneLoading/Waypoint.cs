using SunsetSystems.Utils;
using UnityEngine;

namespace SunsetSystems.Persistence
{
    public class Waypoint : MonoBehaviour
    {
        [field: SerializeField]
        public Transform FaceDirection { get; private set; }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.1f);
            Vector3 xOffset = new(.1f, 0f, 0f);
            Gizmos.DrawLine(transform.position - xOffset, transform.position + xOffset);
            Vector3 zOffset = new(0f, 0f, .1f);
            Gizmos.DrawLine(transform.position - zOffset, transform.position + zOffset);
        }
    }
}
