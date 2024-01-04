using System.Collections.Generic;
using InsaneSystems.RTSSelection;
using Sirenix.OdinInspector;
using SunsetSystems.Entities.Characters.Actions;
using SunsetSystems.Entities.Characters.Interfaces;
using SunsetSystems.Party;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;



namespace SunsetSystems.Input
{
    public class ExplorationInputHandler : SerializedMonoBehaviour, IGameplayInputHandler
    {
        private const int RAYCAST_RANGE = 100;

        [Title("References")]
        [SerializeField]
        private Vector2 mousePosition;
        private Collider lastHit;
        [SerializeField]
        private LayerMask _raycastTargetMask;
        [SerializeField]
        private float _followerStoppingDistance = 1.0f;
        [SerializeField]
        List<ISelectable> selectables;

        public void HandlePointerPosition(InputAction.CallbackContext context)
        {
            if (context.performed is false)
                return;
            mousePosition = context.ReadValue<Vector2>();
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, RAYCAST_RANGE, _raycastTargetMask, QueryTriggerInteraction.Collide))
            {
                if (lastHit == null)
                {
                    lastHit = hit.collider;
                }
                HandleExplorationPointerPosition(hit);
            }

            void HandleExplorationPointerPosition(RaycastHit hit)
            {
                IInteractable interactable;
                if (lastHit != hit.collider)
                {
                    if (lastHit.TryGetComponent(out interactable))
                    {
                        interactable.IsHoveredOver = false;
                    }
                    lastHit = hit.collider;
                }
                if (hit.collider.TryGetComponent(out interactable))
                {
                    interactable.IsHoveredOver = true;
                }
            }
        }

        public void HandlePrimaryAction(InputAction.CallbackContext context)
        {
            if (context.performed is false)
                return;
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, RAYCAST_RANGE, _raycastTargetMask))
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
                    _ = mainCharacter.PerformAction(new Interact(interactable, mainCharacter));
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
            if (context.performed is false)
                return;
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, RAYCAST_RANGE, _raycastTargetMask))
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
                    _ = mainCharacter.PerformAction(new Interact(interactable, mainCharacter));
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
