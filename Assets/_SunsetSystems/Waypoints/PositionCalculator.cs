using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.LevelUtility
{
    public class PositionCalculator
    {
        private readonly int _columns;
        private readonly float _defaultSpacing;
        private readonly IVolume _volume;

        private readonly IPositionResolver _withLeadResolver;
        private readonly IPositionResolver _noLeadResolver;

        public PositionCalculator(int columns, float defaultSpacing, IVolume volume)
        {
            _columns = columns;
            _defaultSpacing = defaultSpacing;
            _volume = volume;

            _withLeadResolver = new LeadPositionStrategy();
            _noLeadResolver = new NoLeadPositionStrategy();
        }

        public IList<Vector3> GetPositions(in int amount, in bool withLead)
        {
            var positionResolver = withLead ? _withLeadResolver : _noLeadResolver;
            return positionResolver.CalculatePositions(in amount, in _columns, in _defaultSpacing, _volume);
        }

        private interface IPositionResolver
        {
            protected const float POSITION_BOUNDS_PADDING = .5f;

            IList<Vector3> CalculatePositions(in int amount, in int columns, in float defaultSpacing, IVolume volume);
        }

        private class NoLeadPositionStrategy : IPositionResolver
        {
            public IList<Vector3> CalculatePositions(in int amount, in int columns, in float defaultSpacing, IVolume volume)
            {
                List<Vector3> result = new();
                if (amount == 1)
                {
                    result.Add(volume.Center);
                    return result;
                }
                Vector3 center = volume.Center;

                int rows = Mathf.CeilToInt((float)amount / columns);

                float spacingX = Mathf.Min(defaultSpacing, (volume.Size.x - 2 * IPositionResolver.POSITION_BOUNDS_PADDING) / (columns - 1));
                float spacingZ = Mathf.Min(defaultSpacing, (volume.Size.z - 2 * IPositionResolver.POSITION_BOUNDS_PADDING) / rows);

                float spacing = Mathf.Min(spacingX, spacingZ);

                float startX = -((columns - 1) * spacing) / 2;
                float startZ = -((rows - 1) * spacing) / 2;

                for (int i = 0; i < amount; i++)
                {
                    int row = i / columns;
                    int col = i % columns;

                    float x = startX + col * spacing;
                    float z = startZ + row * spacing;
                    Vector3 offset = volume.Right * x + volume.Forward * z;
                    result.Add(center + offset);
                }

                return result;
            }
        }

        public class LeadPositionStrategy : IPositionResolver
        {
            public IList<Vector3> CalculatePositions(in int amount, in int columns, in float defaultSpacing, IVolume volume)
            {
                List<Vector3> result = new();
                if (amount == 1)
                {
                    result.Add(volume.Center);
                    return result;
                }
                Vector3 center = volume.Center;

                int adjustedAmount = amount - 1;
                int rows = Mathf.CeilToInt((float)adjustedAmount / columns) + 1; // Add one row for the lead position

                float spacingX = Mathf.Min(defaultSpacing, (volume.Size.x - 2 * IPositionResolver.POSITION_BOUNDS_PADDING) / (columns - 1));
                float spacingZ = Mathf.Min(defaultSpacing, (volume.Size.z - 2 * IPositionResolver.POSITION_BOUNDS_PADDING) / (rows - 1));

                float spacing = Mathf.Min(spacingX, spacingZ);

                float startX = -((columns - 1) * spacing) / 2;
                float startZ = -((rows - 1) * spacing) / 2;

                // Place the lead position at the front
                Vector3 leadPosition = center - volume.Forward * startZ;
                result.Add(leadPosition);

                // Adjust the startZ position for the remaining rows
                startZ += spacing;

                // Generate the remaining positions
                for (int i = 0; i < adjustedAmount; i++)
                {
                    int row = (i / columns) + 1; // Start from the second row
                    int col = i % columns;

                    float x = startX + col * spacing;
                    float z = startZ + (row - 1) * spacing;
                    Vector3 offset = volume.Right * x - volume.Forward * z;
                    result.Add(center + offset);
                }

                return result;
            }
        }
    }

    public interface IVolume
    {
        Vector3 Center { get; }
        Vector3 Size { get; }
        Vector3 Forward { get; }
        Vector3 Right { get; }
        Vector3 Up { get; }
    }

    public class TransformVolume : IVolume
    {
        private readonly Transform _volumeTransform;

        public Vector3 Size { get; }
        public Vector3 Center => _volumeTransform.position;
        public Vector3 Forward => _volumeTransform.forward;
        public Vector3 Right => _volumeTransform.right;
        public Vector3 Up => _volumeTransform.up;

        public TransformVolume(Transform transform, Vector3 size)
        {
            Size = size;
            _volumeTransform = transform;
        }
    }
}
