using Entities.Characters;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InsaneSystems.RTSSelection
{
    public class Selection : MonoBehaviour
    {
        public static readonly List<ISelectable> AllSelectables = new List<ISelectable>();

        public static event Action<List<ISelectable>> OnSelectionStarted, OnSelectionFinished;

        public bool AddModeEnabled { get; set; }
        
        readonly List<ISelectable> selectedObjects = new List<ISelectable>();

        Camera cachedCamera;

        private Vector2 mousePosition;

        void Awake()
        {
            cachedCamera = Camera.main;
            AllSelectables.Clear();
        }

        private void Start()
        {
            foreach (Creature creature in FindObjectsOfType<Creature>())
            {
                ISelectable selectable = creature.GetComponent<ISelectable>();
                if (selectable != null)
                    AllSelectables.Add(selectable);
            }
        }

        /// <summary> Returns all selected objects of type T, which should be derived from ISelectable. If you have only one selectable type, it simply return all of them converted to this type from ISelectable.
        /// <para>If you have several types, you can get filtered selected objects.
        /// For example, if you have SelectableA and SelectableB types (for different actions or something other), calling this method with SelectableA will return only selectables of this type.</para></summary>
        public List<T> GetAllSelectedOfType<T>() where T : ISelectable
        {
            var resultList = new List<T>();
            
            for (var i = 0; i < selectedObjects.Count; i++)
                if (selectedObjects[i] is T)
                    resultList.Add((T)selectedObjects[i]);

            return resultList;
        }

        /// <summary> Returns all selected objects as List of ISelectable. If you need to get specific selectable type, not interface, use GetAllSelectedOfType.</summary>
        public List<ISelectable> GetAllSelected() => selectedObjects;

        /// <summary> Set new camera using this method if your game have some camera changing mechanics, and new camera should work with selection too. </summary>
        public void SetCustomActionCamera(Camera customCamera) => cachedCamera = customCamera;
        
        public void StartSelection() => OnSelectionStarted?.Invoke(selectedObjects);
        
        public void FinishSelection(Vector2 screenStartPoint, Vector2 screenEndPoint)
        {
            if (!AddModeEnabled)
                ClearSelection();
            
            DoMultiselection(screenStartPoint, screenEndPoint);
            DoSingleSelection();
            
            OnSelectionFinished?.Invoke(selectedObjects);
        }
       
        void DoMultiselection(Vector2 screenStartPoint, Vector2 screenEndPoint)
        {
            var selectionRect = new Rect { min = screenStartPoint, max = screenEndPoint };
            
            foreach (ISelectable selectable in AllSelectables)
            {
                var collider = selectable.GetCollider();
                var position = selectable.GetTransform().position;

                if (IsColliderBoundsInScreenRect(position, collider, selectionRect))
                    AddToSelection(selectable);
            }
        }

        bool IsColliderBoundsInScreenRect(Vector3 position, Collider collider, Rect rect)
        {
            var bounds = collider.bounds;
            var e = bounds.extents;
            var c = bounds.center;
            
            var points = new []
            {
                new Vector3(e.x + c.x, c.y, c.z),
                new Vector3(-e.x + c.x, c.y, c.z),
                new Vector3(c.x, e.y + c.y, c.z),
                new Vector3(c.x, -e.y + c.y, c.z),
                new Vector3(c.x, c.y, e.z + c.z),
                new Vector3(c.x, c.y, -e.z + c.z),
            };
            
            var screenPoints = new Vector3[points.Length];
            
            for (var i = 0; i < points.Length; i++)
                screenPoints[i] = cachedCamera.WorldToScreenPoint(points[i]);

            var colliderRect = new Rect();
            
            colliderRect.xMin = colliderRect.xMax = screenPoints[0].x;
            colliderRect.yMin = colliderRect.yMax = screenPoints[0].y;
            
            for (var i = 1; i < screenPoints.Length; i++)
            {
                colliderRect.xMin = Mathf.Min(colliderRect.xMin, screenPoints[i].x);
                colliderRect.xMax = Mathf.Max(colliderRect.xMax, screenPoints[i].x);

                colliderRect.yMin = Mathf.Min(colliderRect.yMin, screenPoints[i].y);
                colliderRect.yMax = Mathf.Max(colliderRect.yMax, screenPoints[i].y);
            }
            
            //DrawDebugSelectionRect(colliderRect);

            return rect.Overlaps(colliderRect, true);
        }

        void DrawDebugSelectionRect(Rect rect)
        {
            var go = new GameObject("UI shit");
            go.transform.SetParent(FindObjectOfType<Canvas>().transform);
            
            var rectTransform = go.AddComponent<RectTransform>();
            rectTransform.sizeDelta = rect.size;
            rectTransform.anchorMin = Vector3.zero;
            rectTransform.anchorMax = Vector3.zero;
            rectTransform.pivot = Vector3.zero;
            rectTransform.anchoredPosition = rect.position;
            
            var image = go.AddComponent<UnityEngine.UI.Image>();
            image.color = new Color(1f, 1f, 0f, 0.25f);
        }
        
        void DoSingleSelection()
        {
            var ray = cachedCamera.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(ray, out var hit, 1000))
            {
                var selectable = hit.collider.GetComponent<ISelectable>();

                if (selectable == null)
                    return;
                
                AddToSelection(selectable);
            }
        }
        
        void AddToSelection(ISelectable selectable)
        {
            if (selectedObjects.Contains(selectable))
            {
                return;
            }
            selectedObjects.Add(selectable);
            selectable.Select();
        }

        void ClearSelection()
        {
            CheckSelectedForNulls();
            
            for (var i = 0; i < selectedObjects.Count; i++)
                selectedObjects[i].Unselect();
            
            selectedObjects.Clear();
        }

        void CheckSelectedForNulls()
        {
            for (int i = selectedObjects.Count - 1; i >= 0; i--)
                if (selectedObjects[i] == null)
                    selectedObjects.RemoveAt(i);
        }

        public void OnMousePosition(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Performed)
                return;
            mousePosition = context.ReadValue<Vector2>();
        }
    }
}