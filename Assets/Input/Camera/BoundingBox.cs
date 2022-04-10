using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Input.CameraControl
{
    public class BoundingBox : MonoBehaviour
    {
        [SerializeField]
        private Vector3 boundingBoxSize = new Vector3(1, 1, 1);
        [SerializeField]
        private Vector3 offset;
        private Color gizmoColor = Color.red;

        public bool IsPositionWithinBounds(Vector3 position)
        {
            bool xBound = IsXWithinBounds(position.x);
            bool yBound = IsYWithinBounds(position.y);
            bool zBound = IsZWithinBounds(position.z);
            return xBound && yBound && zBound;
        }

        private bool IsXWithinBounds(float posX)
        {
            Vector3 boxCeneter = transform.position + offset;
            float minX = boxCeneter.x - Mathf.Abs(boundingBoxSize.x / 2);
            float maxX = boxCeneter.x + Mathf.Abs(boundingBoxSize.x / 2);
            return posX >= minX && posX <= maxX;
        }

        private bool IsYWithinBounds(float posY)
        {
            Vector3 boxCeneter = transform.position + offset;
            float minY = boxCeneter.y - Mathf.Abs(boundingBoxSize.y / 2);
            float maxY = boxCeneter.y + Mathf.Abs(boundingBoxSize.y / 2);
            return posY >= minY && posY <= maxY;
        }

        private bool IsZWithinBounds(float posZ)
        {
            Vector3 boxCenter = transform.position + offset;
            float minZ = boxCenter.z - Mathf.Abs(boundingBoxSize.z / 2);
            float maxZ = boxCenter.z + Mathf.Abs(boundingBoxSize.z / 2);
            return posZ >= minZ && posZ <= maxZ;
        }

        private void OnDrawGizmosSelected()
        {
            Vector3 boundingBoxCenter = transform.position + offset;
            Gizmos.color = gizmoColor;
            Gizmos.DrawWireCube(boundingBoxCenter, boundingBoxSize);
        }
    }
}
