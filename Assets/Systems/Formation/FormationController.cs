using Entities.Characters;
using InsaneSystems.RTSSelection;
using System;
using System.Collections;
using System.Collections.Generic;
using SunsetSystems.Formation.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.AI;

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
                Debug.Log("creating formation element for " + selectable.GetCreature());
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

        public static List<Vector3> GetPositionsFromPoint (Vector3 point)
        {
            List<Vector3> positions = new List<Vector3>();
            //TODO: Placeholder, zmieniæ to kurde
            for (int i = 0; i < 6; i++)
            {
                positions.Add(point);
            }
            return positions;
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
                NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 2.0f, NavMesh.AllAreas);
                transform.position = navHit.position;
                AlignElements();
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
                    NavMesh.SamplePosition(formationElements[i].transform.position, out NavMeshHit navHit, 2.0f, NavMesh.AllAreas);
                    formationElements[i].transform.position = navHit.position;
                }
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
    }
}

