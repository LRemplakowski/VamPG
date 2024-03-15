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
        [field: SerializeField]
        public Transform FaceDirection { get; private set; }

        [Title("Runtime")]
        [ShowInInspector, ReadOnly]
        private bool _registered;
        [ShowInInspector, ReadOnly]
        private static Dictionary<string, Waypoint> _sceneWaypoints = new();

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(_waypointTag))
                _waypointTag = gameObject.name;
        }

        private void Awake()
        {
            if (_sceneWaypoints.TryAdd(_waypointTag, this))
            {
                _registered = true;
                Debug.Log($"Registered waypoint {gameObject}!");
            }
            else
            {
                Debug.LogError($"Could not add waypoint {gameObject} to dictionary! Waypoint tag {_waypointTag} is already present in the dictionary!");
            }
        }

        private void OnDestroy()
        {
            if (_registered)
                _sceneWaypoints.Remove(_waypointTag);
        }

        public static Waypoint GetWaypointByTag(string tag)
        {
            if (_sceneWaypoints.TryGetValue(tag, out Waypoint waypoint))
            {
                return waypoint;
            }
            else
            {
                Debug.LogError($"There is no waypoint with tag {tag} registered in scene!");
                return null;
            }
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
