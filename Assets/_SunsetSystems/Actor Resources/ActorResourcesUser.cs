using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SunsetSystems.Core;
using SunsetSystems.Persistence;
using UnityEngine;

namespace SunsetSystems.ActorResources
{
    public class ActorResourcesUser : SerializedMonoBehaviour, IActionPointUser, IMovementPointUser, IBloodPointUser, IPersistentComponent
    {
        private const string COMPONENT_ID = "ACTOR_RESOURCE_MANAGER";

        [Title("Data")]
        [SerializeField, DictionaryDrawerSettings(IsReadOnly = true, KeyLabel = "Resource", ValueLabel = "Current Amount")]
        private Dictionary<ActorResource, int> _currentResources = new();

        [Title("Runtime")]
        [ShowInInspector, ReadOnly]
        private readonly HashSet<string> _movementBlockers = new();
        [ShowInInspector, ReadOnly]
        private readonly HashSet<string> _apBlockers = new();
        [ShowInInspector, ReadOnly]
        private readonly HashSet<string> _bloodBlockers = new();

        public string ComponentID => COMPONENT_ID;

        private void OnValidate()
        {
            _currentResources ??= new();
            foreach (ActorResource resource in Enum.GetValues(typeof(ActorResource)))
            {
                if (_currentResources.ContainsKey(resource) is false)
                {
                    _currentResources[resource] = 0;
                }
            }
        }

        #region IActionPointUser
        public bool CanUseActionPoints()
        {
            bool result = true;
            result &= !HasAnyBlockers(_apBlockers);
            result &= HasResourceRemaining(_currentResources, ActorResource.ActionPoints);
            return result;
        }

        public int GetCurrentActionPoints()
        {
            return _currentResources[ActorResource.ActionPoints];
        }

        public bool UseActionPoints(int amount)
        {
            if (_currentResources.TryGetValue(ActorResource.ActionPoints, out var currentAP) && currentAP >= amount)
            {
                _currentResources[ActorResource.ActionPoints] -= amount;
                return true;
            }
            return false;
        }

        public void AddActionPointUseBlocker(string sourceID)
        {
            _apBlockers.Add(sourceID);
        }

        public void RemoveActionPointUseBlocker(string sourceID)
        {
            _apBlockers.Remove(sourceID);
        }
        #endregion

        #region IMovementPointUser
        public bool GetCanMove()
        {
            bool result = true;
            result &= !HasAnyBlockers(_movementBlockers);
            result &= HasResourceRemaining(_currentResources, ActorResource.ActionPoints);
            return result;
        }

        public int GetCurrentMovementPoints()
        {
            return ActionPointsToMovementPoints(_currentResources[ActorResource.ActionPoints]);
        }

        public bool GetCanUseBloodPoints()
        {
            bool result = true;
            result &= !HasAnyBlockers(_bloodBlockers);
            result &= HasResourceRemaining(_currentResources, ActorResource.BloodPoints);
            return result;
        }

        public bool UseMovementPoints(int amount)
        {
            if (_currentResources.TryGetValue(ActorResource.ActionPoints, out var currentAP) && ActionPointsToMovementPoints(currentAP) >= amount)
            {
                _currentResources[ActorResource.ActionPoints] -= MovementPointsToActionPoints(amount);
                return true;
            }
            return false;
        }

        public void AddMovementPointUseBlocker(string sourceID)
        {
            _movementBlockers.Add(sourceID);
        }

        public void RemoveMovementPointUseBlocker(string sourceID)
        {
            _movementBlockers.Remove(sourceID);
        }
        #endregion

        #region IBloodPointUser
        public int GetCurrentBloodPoints()
        {
            return _currentResources[ActorResource.BloodPoints];
        }

        public bool UseBloodPoints(int amount)
        {
            if (_currentResources.TryGetValue(ActorResource.ActionPoints, out var currentBP) && currentBP >= amount)
            {
                _currentResources[ActorResource.BloodPoints] -= amount;
                return true;
            }
            return false;
        }

        public void AddBloodPointUseBlocker(string sourceID)
        {
            _bloodBlockers.Add(sourceID);
        }

        public void RemoveBloodPointUseBlocker(string sourceID)
        {
            _bloodBlockers.Remove(sourceID);
        }
        #endregion

        private static bool HasAnyBlockers(ICollection<string> blockerCollection) => blockerCollection.Count > 0;

        private static bool HasResourceRemaining(Dictionary<ActorResource, int> resourceMap, ActorResource resource)
        {
            return resourceMap.TryGetValue(resource, out var remaining) && remaining > 0;
        }

        private static int ActionPointsToMovementPoints(int ap) => ap * GameConstants.MOVEMENT_PER_AP;

        private static int MovementPointsToActionPoints(int mp)
        {
            int ap = mp / GameConstants.MOVEMENT_PER_AP;
            ap += mp % GameConstants.MOVEMENT_PER_AP > 0 ? 1 : 0;
            return ap;
        }

        #region IPersistentComponent
        public object GetComponentPersistenceData()
        {
            return new ResourcePersistentData(this);
        }

        public void InjectComponentPersistenceData(object data)
        {
            if (data is not ResourcePersistentData resourceData)
                return;
            _currentResources = new(resourceData.CurrentResources);
        }

        [Serializable]
        public class ResourcePersistentData
        {
            public Dictionary<ActorResource, int> CurrentResources;

            public ResourcePersistentData(ActorResourcesUser resourceManager)
            {
                CurrentResources = new(resourceManager._currentResources);
            }

            public ResourcePersistentData()
            {
                CurrentResources = new();
            }
        }
        #endregion
    }
}
