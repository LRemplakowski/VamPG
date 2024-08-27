using System.Collections.Generic;
using System.Linq;
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
            Objective.OnObjectiveActive += StartTracking;
            Objective.OnObjectiveCompleted += PartCompleted;
        }

        private void OnDisable()
        {
            Objective.OnObjectiveActive -= StartTracking;
            Objective.OnObjectiveCompleted -= PartCompleted;
        }

        private void StartTracking(Objective objective)
        {
            _objectiveActive = true;
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
            if (_compositeParts.Any(ob => ob.DatabaseID == objective.DatabaseID))
                _completedCount++;
        }

        public bool CheckCompletion(Objective objective)
        {
            return _completedCount >= _compositeParts.Count;
        }
    }
}
