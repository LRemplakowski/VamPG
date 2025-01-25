using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.LevelUtility
{
    public class PositionCalculator
    {
        private const float POSITION_BOUNDS_PADDING = .5f;

        private readonly int _columns;
        private readonly float _defaultSpacing;
        private readonly Bounds _volume;

        private readonly IPositionCalculationStrategy _withLeadResolver;
        private readonly IPositionCalculationStrategy _noLeadResolver;

        public PositionCalculator(int columns, float defaultSpacing, Bounds volume)
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
            return positionResolver.CalculatePositions(in amount, in _columns, in _defaultSpacing, in _volume);
        }

        private interface IPositionCalculationStrategy
        {
            IList<Vector3> CalculatePositions(in int amount, in int columns, in float defaultSpacing, in Bounds volume);
        }

        private class NoLeadPositionStrategy : IPositionCalculationStrategy
        {
            public IList<Vector3> CalculatePositions(in int amount, in int columns, in float defaultSpacing, in Bounds volume)
            {
                List<Vector3> result = new();
                if (amount == 1)
                {
                    result.Add(volume.center);
                    return result;
                }
                Vector3 center = volume.center;

                int rows = Mathf.CeilToInt((float)amount / columns);

                float spacingX = Mathf.Min(defaultSpacing, (volume.size.x - 2 * POSITION_BOUNDS_PADDING) / (columns - 1));
                float spacingZ = Mathf.Min(defaultSpacing, (volume.size.z - 2 * POSITION_BOUNDS_PADDING) / rows);

                float spacing = Mathf.Min(spacingX, spacingZ);

                float startX = center.x - ((columns - 1) * spacing) / 2;
                float startZ = center.z - ((rows - 1) * spacing) / 2;

                for (int i = 0; i < amount; i++)
                {
                    int row = i / columns;
                    int col = i % columns;

                    float x = startX + col * spacing;
                    float z = startZ + row * spacing;
                    result.Add(new Vector3(x, center.y, z));
                }

                return result;
            }
        }

        public class LeadPositionStrategy : IPositionCalculationStrategy
        {
            public IList<Vector3> CalculatePositions(in int amount, in int columns, in float defaultSpacing, in Bounds volume)
            {
                List<Vector3> result = new();
                if (amount == 1)
                {
                    result.Add(volume.center);
                    return result;
                }
                Vector3 center = volume.center;

                int adjustedAmount = amount - 1;
                int rows = Mathf.CeilToInt((float)adjustedAmount / columns) + 1; // Add one row for the lead position

                float spacingX = Mathf.Min(defaultSpacing, (volume.size.x - 2 * POSITION_BOUNDS_PADDING) / (columns - 1));
                float spacingZ = Mathf.Min(defaultSpacing, (volume.size.z - 2 * POSITION_BOUNDS_PADDING) / (rows - 1));

                float spacing = Mathf.Min(spacingX, spacingZ);

                float startX = center.x - ((columns - 1) * spacing) / 2;
                float startZ = center.z - ((rows - 1) * spacing) / 2;

                // Place the lead position at the front
                Vector3 leadPosition = new Vector3(center.x, center.y, startZ);
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
                    result.Add(new Vector3(x, center.y, z));
                }

                return result;
            }
        }
    }
}
