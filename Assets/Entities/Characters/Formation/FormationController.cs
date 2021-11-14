using InsaneSystems.RTSSelection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Entities.Characters.Formation
{
    public class FormationController : InputHandler<FormationController>
    {
        [SerializeField]
        private FormationElement formationElementPrefab;
        //[SerializeField]
        //private FormationSlot slotPrefab;
        private List<FormationElement> formationElements = new List<FormationElement>();

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

        public override void Initialize()
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
            if (!context.performed || EventSystem.current.IsPointerOverGameObject())
                return;
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, raycastRange, defaultRaycastMask))
            {
                AlignElements();
                transform.position = hit.point;
                isDirty = true;
            }
        }

        private void AlignElements()
        {
            for (int i = 0; i < formationElements.Count; i++)
            {
                formationElements[i].transform.localPosition = new Vector3(i + 1f, 0.1f, 0);
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

