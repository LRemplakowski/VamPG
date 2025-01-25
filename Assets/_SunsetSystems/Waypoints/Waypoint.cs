using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SunsetSystems.LevelUtility
{
    [RequireComponent(typeof(BoxCollider))]
    public class Waypoint : SerializedMonoBehaviour
    {
        [Title("Config")]
        [SerializeField]
        private string _waypointTag = "";
        [SerializeField, MinValue(1)]
        private int _waypointCapacity = 3;
        [SerializeField, MinValue(1)]
        private int _columns = 2;
        [SerializeField]
        private bool _alwaysHaveLeadCharacter;

        [SerializeField, HideInInspector]
        private Collider _waypointSpawnVolume;
        
        public string WaypointTag => _waypointTag;
        [field: SerializeField]
        public Transform FaceDirection { get; private set; }



        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(_waypointTag))
            {
                _waypointTag = name;
            }
            if (_waypointSpawnVolume == null)
            {
                _waypointSpawnVolume = GetComponent<Collider>();
            }
            _waypointSpawnVolume.isTrigger = true;
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
