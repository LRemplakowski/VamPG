using Sirenix.OdinInspector;
using SunsetSystems.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using SunsetSystems.Entities.Characters;
using InsaneSystems.RTSSelection;
using SunsetSystems.Combat;
using SunsetSystems.Utils.Input;
using System;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using SunsetSystems.Party;
using SunsetSystems.Spellbook;
using SunsetSystems.Entities.Characters.Interfaces;
using SunsetSystems.Entities.Characters.Actions;
using SunsetSystems.Combat.Grid;
using SunsetSystems.Entities.Interfaces;



namespace SunsetSystems.Input
{
    public class ExplorationInputHandler : SerializedMonoBehaviour, IGameplayInputHandler
    {
        [Title("References")]
        [SerializeField]
        private Vector2 mousePosition;
        private Collider lastHit;
        private const int raycastRange = 100;
        [SerializeField]
        private LayerMask _raycastTargetMask;
        [SerializeField]
        private float _followerStoppingDistance = 1.0f;

        public void HandlePointerPosition(InputAction.CallbackContext context)
        {
            mousePosition = context.ReadValue<Vector2>();
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, raycastRange, _raycastTargetMask, QueryTriggerInteraction.Ignore))
            {
                if (lastHit == null)
                {
                    lastHit = hit.collider;
                }
                HandleExplorationPointerPosition(hit);
            }
            void HandleExplorationPointerPosition(RaycastHit hit)
            {
                IInteractable interactable = null;
                if (lastHit != hit.collider)
                {
                    interactable = lastHit.gameObject
                        .GetComponents<IInteractable>()?
                        .FirstOrDefault(interactable => (interactable as MonoBehaviour).enabled);
                    if (interactable != null)
                    {
                        interactable.IsHoveredOver = false;
                        interactable = null;
                    }
                    lastHit = hit.collider;
                }
                interactable = lastHit.gameObject
                    .GetComponents<IInteractable>()?
                    .FirstOrDefault(interactable => (interactable as MonoBehaviour).enabled);
                if (interactable != null)
                {
                    interactable.IsHoveredOver = true;
                }
            }
        }

        public void HandlePrimaryAction(InputAction.CallbackContext context)
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, raycastRange, _raycastTargetMask))
            {
                HandleNoSelectionExplorationInput();
            }

            void HandleNoSelectionExplorationInput()
            {
                ICreature mainCharacter = PartyManager.Instance.MainCharacter;
                if (mainCharacter == null)
                    return;
                // Main Character should always take the lead since it's a first entry in ActiveParty list
                List<ICreature> creatures = new();
                if (hit.collider.TryGetComponent(out IInteractable interactable))
                {
                    mainCharacter.PerformAction(new Interact(interactable, mainCharacter));
                    creatures.Add(null);
                }
                else
                {
                    creatures.Add(PartyManager.Instance.MainCharacter);
                }
                if (PartyManager.Instance.ActiveParty.Count > 1)
                    creatures.AddRange(PartyManager.Instance.Companions);
                MoveCreaturesToPosition(creatures, hit.point);
            }
        }

        public void HandleSecondaryAction(InputAction.CallbackContext context)
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, raycastRange, _raycastTargetMask))
            {
                HandleSelectionExplorationInput();
            }
            else
            {
                HandleNoSelectionExplorationInput();
            }

            void HandleSelectionExplorationInput()
            {
                List<ISelectable> selectables = Selection.Instance.GetAllSelected();
                ICreature currentLead;
                if (selectables.Count > 0)
                {
                    currentLead = selectables[0].GetCreature();
                }
                else
                {
                    return;
                }
                if (hit.collider.gameObject.TryGetComponent(out IInteractable interactable))
                {
                    currentLead.PerformAction(new Interact(interactable, currentLead));
                }
                else
                {
                    MoveCurrentSelectionToPositions(hit);
                }
            }

            void HandleNoSelectionExplorationInput()
            {
                ICreature mainCharacter = PartyManager.Instance.MainCharacter;
                if (mainCharacter == null)
                    return;
                // Main Character should always take the lead since it's a first entry in ActiveParty list
                List<ICreature> creatures = new();
                IInteractable interactable = hit.collider.gameObject
                        .GetComponents<IInteractable>()?
                        .FirstOrDefault(interactable => (interactable as MonoBehaviour).enabled);
                if (interactable != null)
                {
                    mainCharacter.PerformAction(new Interact(interactable, mainCharacter));
                    creatures.Add(null);
                }
                else
                {
                    creatures.Add(PartyManager.Instance.MainCharacter);
                }
                if (PartyManager.Instance.ActiveParty.Count > 1)
                    creatures.AddRange(PartyManager.Instance.Companions);
                MoveCreaturesToPosition(creatures, hit.point);
            }
        }
        private void MoveCurrentSelectionToPositions(RaycastHit hit)
        {
            List<ICreature> allSelected = Selection.Instance.GetAllSelected().Select(s => s.GetCreature()) as List<ICreature>;
            MoveCreaturesToPosition(allSelected, hit.point);
            Debug.Log("Moved Creature");
        }

        private void MoveCreaturesToPosition(List<ICreature> creatures, Vector3 samplingPoint)
        {
            float stoppingDistance = 0f;
            for (int i = 0; i < creatures.Count; i++)
            {
                NavMesh.SamplePosition(samplingPoint, out NavMeshHit hit, 2.0f, NavMesh.AllAreas);
                stoppingDistance += (i % 2) * _followerStoppingDistance;
                ICreature creature = creatures[i];
                if (creature != null)
                {
                    creature.PerformAction(new Move(creature, hit.position, stoppingDistance), true);
                    Debug.Log("Moved Creature");
                }
            }
        }
    }
}
