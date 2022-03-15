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
        public static FormationData FormationData { get; set; }

        private Vector2 mousePosition;
        private const int raycastRange = 100;

        [SerializeField]
        private LayerMask defaultRaycastMask;

        private static List<ISelectable> currentSelection;

        private void OnEnable()
        {
            Selection.OnSelectionFinished += OnSelectionFinished;
        }

        private void OnDisable()
        {
            Selection.OnSelectionFinished -= OnSelectionFinished;
        }

        private void OnSelectionFinished(List<ISelectable> selectedObjects)
        {
            currentSelection = selectedObjects;
        }

        public static List<Vector3> GetPositionsFromPoint(Vector3 point)
        {
            List<Vector3> positions = new List<Vector3>();
            for (int i = 0; i < currentSelection.Count; i++)
            {
                Vector3 positionOffset = FormationData.positions[i];
                Vector3 position = point + positionOffset;
                positions.Add(position);
            }
            return positions;
        }

        public static Vector3 GetPositionInFormation(Vector3 formationPoint, int index)
        {
            Vector3 position = new Vector3(formationPoint.x + FormationData.positions[index].x, 0, formationPoint.z + FormationData.positions[index].z);
            return position;
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
                MoveCurrentSelectionToPositions(hit);
            }
        }

        private void MoveCurrentSelectionToPositions(RaycastHit hit)
        {
            Vector3 samplingPoint;
            for (int i = 0; i < currentSelection.Count; i++)
            {
                Vector3 positionOffset = FormationData.positions[i];
                samplingPoint = hit.point + positionOffset;
                NavMesh.SamplePosition(samplingPoint, out NavMeshHit navHit, 2.0f, NavMesh.AllAreas);
                MoveSelectableToPosition(currentSelection[i], navHit.position);
            }
        }

        private void MoveSelectableToPosition(ISelectable selectable, Vector3 position)
        {
            Creature creature = selectable.GetCreature();
            creature.ClearAllActions();
            creature.Move(position);
        }

        public void OnMousePosition(InputAction.CallbackContext context)
        {
            if (context.performed)
                mousePosition = context.ReadValue<Vector2>();
        }
    }
}

