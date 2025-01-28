using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SunsetSystems.LevelUtility
{
    public class Waypoint : SerializedMonoBehaviour, IWaypoint
    {
        [Title("Config")]
        [SerializeField]
        private string _waypointTag = "";
        [SerializeField, MinValue(1)]
        private int _columns = 2;
        [SerializeField, MinValue(.1)]
        private float _defaultSpacing = .75f;
        [SerializeField]
        private Vector3 _spawnVolume = new(2, 1, 3);
        [SerializeField]
        private bool _alwaysHaveLeadCharacter;
        [Title("Editor")]
        [InfoBox("Shows how characters will be spread during spawn for given amount of characters.")]
        [ShowInInspector, MinValue(1)]
        private int _spawnDistribution = 5;

        public string WaypointTag => _waypointTag;

        private PositionCalculator _positionCalculator;

        private void Awake()
        {
            EnsureCalculator();
        }

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(_waypointTag))
            {
                _waypointTag = name;
            }
            _positionCalculator = new(_columns, _defaultSpacing, new(transform, _spawnVolume));
        }

        public Vector3 GetPosition() => GetPositions(1).First();

        public IList<Vector3> GetPositions(int amount)
        {
            return CalculateSpawnPositions(in amount, in _alwaysHaveLeadCharacter, _positionCalculator);
        }

        public Vector3 GetFacingDirection() => GetFacingDirectionFromPosition(GetPosition());

        public Vector3 GetFacingDirectionFromPosition(in Vector3 position)
        {
            return position + transform.forward;
        }

        private static IList<Vector3> CalculateSpawnPositions(in int amount, in bool forceLeadPosition, PositionCalculator positionCalculator)
        {
            bool hasLeadPosition = amount % 2 == 1 || forceLeadPosition;
            return positionCalculator.GetPositions(in amount, in hasLeadPosition);
        }

        private void EnsureCalculator()
        {
            _positionCalculator ??= new(_columns, _defaultSpacing, new(transform, _spawnVolume));
        }

#if UNITY_EDITOR
        [UnityEditor.DrawGizmo(UnityEditor.GizmoType.NonSelected | UnityEditor.GizmoType.Pickable)]
        private static void OnDrawGizmosNotSelected(Waypoint wp, UnityEditor.GizmoType gizmoType)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(wp.transform.position, .25f);
        }

        private void OnDrawGizmosSelected()
        {
            EnsureCalculator();
            DrawSpawnVolume();
            DrawPositionExamples();

            void DrawSpawnVolume()
            {
                Gizmos.color = Color.green;
                Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
                Gizmos.matrix = rotationMatrix;
                Gizmos.DrawWireCube(Vector3.zero, _spawnVolume);
            }

            void DrawPositionExamples()
            {
                Gizmos.color = Color.red;
                Gizmos.matrix = Matrix4x4.identity;
                foreach (var position in GetPositions(_spawnDistribution))
                {
                    Gizmos.DrawWireSphere(position, 0.5f);
                }
            }
        }
#endif
    }
}
