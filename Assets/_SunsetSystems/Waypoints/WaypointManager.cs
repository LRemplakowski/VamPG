using System.Collections.Generic;
using Sirenix.OdinInspector;
using SunsetSystems.Game;
using SunsetSystems.LevelUtility;
using UnityEngine;

namespace SunsetSystems.Core
{
    public class WaypointManager : SerializedMonoBehaviour
    {
        public static WaypointManager Instance { get; private set; }

        [SerializeField, Required]
        private Waypoint _defaultSceneWaypoint;
        [SerializeField, ReadOnly]
        private Dictionary<string, Waypoint> _sceneWaypoints = new();

        private static string _waypointOverride = "";

        [Button("Force Validate")]
        private void OnValidate()
        {
            _sceneWaypoints ??= new();
            _sceneWaypoints.Clear();
            var sceneWpts = FindObjectsByType<Waypoint>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var wp in sceneWpts)
            {
                _sceneWaypoints[wp.WaypointTag] = wp;
            }
        }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            GameManager.Instance.OnLevelStart += ClearWaypointOverride;
        }

        public Waypoint GetSceneDefaultEntryWaypoint()
        {
            if (string.IsNullOrWhiteSpace(_waypointOverride) is false && _sceneWaypoints.TryGetValue(_waypointOverride, out var waypointOverride))
                return waypointOverride;
            else
                return _defaultSceneWaypoint;
        }

        public Waypoint GetWaypointByTag(string tag)
        {
            if (_sceneWaypoints.TryGetValue(tag, out var waypoint))
                return waypoint;
            Debug.LogError($"There is no waypoint with tag {tag} in scene {gameObject.scene}! Returning default waypoint!");
            return _defaultSceneWaypoint;
        }

        public void OverrideSceneWaypoint(string waypointTag)
        {
            _waypointOverride = waypointTag;
        }

        public void ClearWaypointOverride()
        {
            _waypointOverride = "";
        }
    }
}
