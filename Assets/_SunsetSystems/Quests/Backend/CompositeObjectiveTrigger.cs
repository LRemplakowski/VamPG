using SunsetSystems.Journal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Journal
{
    public class CompositeObjectiveTrigger : MonoBehaviour, IObjectiveTrigger
    {
        [SerializeField]
        private Objective _compositeObjective;
        [SerializeField]
        private List<Objective> _compositeParts;

        private int _completedCount;
        private bool _objectiveActive;

        private void OnEnable()
        {
            _compositeObjective.OnObjectiveActive += StartTracking;
        }

        private void OnDisable()
        {
            _compositeObjective.OnObjectiveActive -= StartTracking;
        }

        private void StartTracking(Objective objective)
        {
            _objectiveActive = true;
        }

        private void Start()
        {
            _compositeParts.ForEach(cp => cp.OnObjectiveCompleted += PartCompleted);
        }

        private void Update()
        {
            if (_objectiveActive && CheckCompletion(_compositeObjective))
            {
                _compositeObjective.Complete();
                gameObject.SetActive(false);
            }
        }

        private void PartCompleted(Objective objective)
        {
            _completedCount++;
            objective.OnObjectiveCompleted -= PartCompleted;
        }

        public bool CheckCompletion(Objective objective)
        {
            return _completedCount >= _compositeParts.Count;
        }
    }
}
