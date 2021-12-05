using Entities.Characters;
using InsaneSystems.RTSSelection;
using System;
using System.Collections;
using System.Collections.Generic;
using SunsetSystems.Formation.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace SunsetSystems.Formation
{
    public class FormationController : InputHandler
    {
        [SerializeField]
        private FormationElement formationElementPrefab;
        //[SerializeField]
        //private FormationSlot slotPrefab;
        private List<FormationElement> formationElements = new List<FormationElement>();
        public FormationData FormationData { get; set; }

        private Vector2 mousePosition;
        private const int raycastRange = 100;

        private bool isDirty = false;
        [SerializeField]
        private LayerMask defaultRaycastMask;

        //[SerializeField]
        //private int x = 100, y = 100;

        //private FormationSlot[,] formationGrid;

        private void OnEnable()
        {
            Selection.OnSelectionFinished += OnSelectionFinished;
        }

        private void OnDisable()
        {
            Selection.OnSelectionFinished -= OnSelectionFinished;
        }

        private void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            formationElements = new List<FormationElement>();
        }

        private void OnSelectionFinished(List<ISelectable> selectedObjects)
        {
            CreateFormation(selectedObjects);
        }

        private void CreateFormation(List<ISelectable> selectables)
        {
            ClearFormation();
            foreach (ISelectable selectable in selectables)
            {
                CreateFormationElement(selectable.GetCreature());
            }
        }

        private void CreateFormationElement(Creature creature)
        {
            FormationElement formationElement = Instantiate(formationElementPrefab, this.transform);
            formationElement.character = creature;
            formationElement.transform.position = new Vector3(creature.transform.position.x, 0.1f, creature.transform.position.z);
            formationElements.Add(formationElement);
        }

        private void ClearFormation()
        {
            formationElements.ForEach(e => Destroy(e.gameObject));
            formationElements.Clear();
        }

        public void OnRightClick(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            Debug.LogWarning("Handling mouse click");
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, raycastRange, defaultRaycastMask))
            {
                Debug.LogWarning("Raycast hit!");
                AlignElements();
                transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                isDirty = true;
            }
        }

        private void AlignElements()
        {
            try
            {
                for (int i = 0; i < formationElements.Count; i++)
                {
                    formationElements[i].transform.localPosition = FormationData.positions[i];
                }
                Debug.LogError("Formation positions set!");
            }
            catch (IndexOutOfRangeException e)
            {
                Debug.LogException(e);
                foreach (FormationElement element in formationElements)
                {
                    element.transform.position = Vector3.zero;
                }
            }
        }

        public void OnMousePosition(InputAction.CallbackContext context)
        {
            if (context.performed)
                mousePosition = context.ReadValue<Vector2>();
        }

        private void FixedUpdate()
        {
            if (isDirty)
            {
                formationElements.ForEach(e => e.MoveOwnerToElement());
                isDirty = false;
            }  
        }

        //[ContextMenu("Generate grid")]
        //private void CreateFormationGrid()
        //{
        //    ClearFormationGrid();
        //    formationGrid = new FormationSlot[x, y];
        //    for (int i = 0; i < x; i++)
        //    {
        //        for (int j = 0; j < y; j++)
        //        {
        //            formationGrid[i, j] = Instantiate(slotPrefab, this.transform);
        //        }
        //    }
        //}

        //[ContextMenu("Clear grid")]
        //private void ClearFormationGrid()
        //{
        //    foreach (FormationSlot slot in GetComponentsInChildren<FormationSlot>())
        //        DestroyImmediate(slot.gameObject);
        //}
    }
}

