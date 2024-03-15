using Sirenix.OdinInspector;
using SunsetSystems.LevelUtility;
using UnityEngine;

namespace SunsetSystems
{
    public class WaypointManager : MonoBehaviour
    {
        public static WaypointManager Instance { get; private set; }

        [SerializeField, Required]
        private Waypoint _defaultSceneWaypoint;

        private static Waypoint _waypointOverride;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        public Waypoint GetSceneEntryWaypoint()
        {
            if (_waypointOverride != null)
                return _waypointOverride;
            else
                return _defaultSceneWaypoint;
        }

        public void OverrideSceneWaypoint(string waypointTag)
        {
            _waypointOverride = Waypoint.GetWaypointByTag(waypointTag);
        }

        public void ClearWaypointOverride()
        {
            _waypointOverride = null;
        }
    }
}
