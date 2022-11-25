using SunsetSystems.Utils;
using UnityEngine;

namespace SunsetSystems.Input.CameraControl
{
    [RequireComponent(typeof(Tagger))]
    public class BoundingBox : MonoBehaviour
    {
        [SerializeField]
        private Vector3 _boundingBoxSize = new Vector3(1, 1, 1);
        [SerializeField]
        private Vector3 _offset;
        [SerializeField]
        private Color _gizmoColor = Color.red;

        public bool IsPositionWithinBounds(Vector3 position)
        {
            bool xBound = IsXWithinBounds(position.x);
            bool yBound = IsYWithinBounds(position.y);
            bool zBound = IsZWithinBounds(position.z);
            return xBound && yBound && zBound;
        }

        public Vector3 ClampPositionToBounds(Vector3 position)
        {
            return new Vector3
            {
                x = Mathf.Clamp(position.x, GetMinX(), GetMaxX()),
                y = Mathf.Clamp(position.y, GetMinY(), GetMaxY()),
                z = Mathf.Clamp(position.z, GetMinZ(), GetMaxZ())
            };
        }

        private bool IsXWithinBounds(float posX)
        {
            float minX = GetMinX();
            float maxX = GetMaxX();
            return posX >= minX && posX <= maxX;
        }

        private bool IsYWithinBounds(float posY)
        {
            float minY = GetMinY();
            float maxY = GetMaxY();
            return posY >= minY && posY <= maxY;
        }

        private bool IsZWithinBounds(float posZ)
        {
            float minZ = GetMinZ();
            float maxZ = GetMaxZ();
            return posZ >= minZ && posZ <= maxZ;
        }

        private float GetMaxX()
        {
            return transform.position.x + _offset.x + Mathf.Abs(_boundingBoxSize.x / 2);
        }

        private float GetMinX()
        {
            return transform.position.x + _offset.x - Mathf.Abs(_boundingBoxSize.x / 2);
        }

        private float GetMaxY()
        {
            return transform.position.y + _offset.y + Mathf.Abs(_boundingBoxSize.y / 2);
        }

        private float GetMinY()
        {
            return transform.position.y + _offset.y - Mathf.Abs(_boundingBoxSize.y / 2);
        }

        private float GetMaxZ()
        {
            return transform.position.z + _offset.z + Mathf.Abs(_boundingBoxSize.z / 2);
        }

        private float GetMinZ()
        {
            return transform.position.z + _offset.z - Mathf.Abs(_boundingBoxSize.z / 2);
        }

        private void OnDrawGizmos()
        {
            Vector3 boundingBoxCenter = transform.position + _offset;
            Gizmos.color = _gizmoColor;
            Gizmos.DrawWireCube(boundingBoxCenter, _boundingBoxSize);
        }
    }
}
