using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SunsetSystems.Utils
{
    public class EnviromentScroller : SerializedMonoBehaviour
    {
        [Title("Config")]
        [SerializeField]
        private float _scrollSpeed = 10f;
        [SerializeField]
        private ScrollDirection _scrollDirection;
        [SerializeField]
        private float _scrollChunkSize = 100f;
        [SerializeField]
        private Vector3 _scrollCenterOffset = Vector3.zero;
        [Title("References")]
        [SerializeField]
        private List<Transform> _scrollTransforms = new();

        private Vector3 ScrollCenter => transform.position + _scrollCenterOffset;

        private void Update()
        {
            var direction = ScrollDirectionToVector(_scrollDirection);
            var timeDelta = Time.deltaTime;
            foreach (Transform scrolled in _scrollTransforms)
            {
                scrolled.localPosition += _scrollSpeed * timeDelta * direction;
                if (CheckShouldMoveToScrollFront(scrolled.localPosition))
                    MoveToFront(scrolled);
            }
        }

        private void MoveToFront(Transform scrolled)
        {
            scrolled.localPosition += _scrollChunkSize * _scrollTransforms.Count * -ScrollDirectionToVector(_scrollDirection);
        }

        private bool CheckShouldMoveToScrollFront(Vector3 localPosition)
        {
            return _scrollDirection switch
            {
                ScrollDirection.Forward => (localPosition.z + _scrollCenterOffset.z) >= GetScrollEndPoint().z,
                ScrollDirection.Backward => (localPosition.z + _scrollCenterOffset.z) <= GetScrollEndPoint().z,
                ScrollDirection.Left => (localPosition.x + _scrollCenterOffset.x) <= GetScrollEndPoint().x,
                ScrollDirection.Right => (localPosition.x + _scrollCenterOffset.x) >= GetScrollEndPoint().x,
                _ => false,
            };
        }

        private Vector3 GetScrollStartPoint()
        {
            Vector3 scrollDirection = -ScrollDirectionToVector(_scrollDirection);
            Vector3 result = _scrollCenterOffset + ((_scrollTransforms.Count - 1) / 2 * scrollDirection * _scrollChunkSize);
            return result;
        }

        private Vector3 GetScrollEndPoint()
        {
            Vector3 scrollDirection = ScrollDirectionToVector(_scrollDirection);
            Vector3 result = _scrollCenterOffset + ((_scrollTransforms.Count - 1) / 2 * scrollDirection * _scrollChunkSize);
            return result;
        }    

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(ScrollCenter, 1f);
            var scrollVector = ScrollDirectionToVector(_scrollDirection);
            Gizmos.DrawWireCube(ScrollCenter, (scrollVector * (_scrollChunkSize * (_scrollTransforms.Count - 1))) + (Vector3.one - scrollVector));
            Gizmos.color = Color.green;
            Gizmos.DrawCube(transform.position + GetScrollEndPoint(), Vector3.one * 2);
            Gizmos.DrawCube(transform.position + GetScrollStartPoint(), Vector3.one * 2);
        }

        private Vector3 ScrollDirectionToVector(ScrollDirection direction)
        {
            return direction switch
            {
                ScrollDirection.Forward => Vector3.forward,
                ScrollDirection.Backward => Vector3.back,
                ScrollDirection.Left => Vector3.left,
                ScrollDirection.Right => Vector3.right,
                _ => Vector3.zero,
            };
        }

        private enum ScrollDirection
        {
            Forward, Backward, Left, Right
        }
    }
}
