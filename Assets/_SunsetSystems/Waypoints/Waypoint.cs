using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SunsetSystems.LevelUtility
{
    public class Waypoint : MonoBehaviour
    {
        [Title("Config")]
        [SerializeField]
        private string _waypointTag = "";
        public string WaypointTag => _waypointTag;
        [field: SerializeField]
        public Transform FaceDirection { get; private set; }

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(_waypointTag))
                _waypointTag = name;
        }

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
