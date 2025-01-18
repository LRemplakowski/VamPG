using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SunsetSystems.Combat;
using SunsetSystems.ActionSystem;
using UnityEngine;
using SunsetSystems.Combat.Grid;
using System;

namespace SunsetSystems.Playables
{
    public class PlayableActionSequence : SerializedMonoBehaviour
    {
        [SerializeField]
        private IActionPerformer _actionPerformer;
        [SerializeField]
        private bool _loop;
        [SerializeField]
        private bool _playOnStart;
        [SerializeField, ListDrawerSettings(HideAddButton = true, HideRemoveButton = false)]
        private List<EntityAction> _actionSequence = new();

        private IEnumerator _sequenceCoroutine;

        private void Start()
        {
            if (_playOnStart)
                StartSequence();
        }

        private void Update()
        {
            if (_loop && _sequenceCoroutine == null)
                StartSequence();
        }

        public void StartSequence()
        {
            if (_sequenceCoroutine != null)
                return;
            _sequenceCoroutine = ActionSequence();
            StartCoroutine(_sequenceCoroutine);
        }

        private IEnumerator ActionSequence()
        {
            foreach (var action in _actionSequence)
            {
                if (_actionPerformer.HasActionsQueued)
                    yield return new WaitUntil(() => _actionPerformer.HasActionsQueued is false);
                _actionPerformer.PerformAction(action, false);
            }
            if (_actionPerformer.HasActionsQueued)
                yield return new WaitUntil(() => _actionPerformer.HasActionsQueued is false);
            var actionsClone = new List<EntityAction>();
            foreach (var action in _actionSequence)
            {
                if (action is ICloneable cloneable)
                {
                    actionsClone.Add(cloneable.Clone() as EntityAction);
                }
            }
            _actionSequence.Clear();
            _actionSequence.AddRange(actionsClone);
            _sequenceCoroutine = null;
        }

        public void StopSequence()
        {
            if (_sequenceCoroutine != null)
            {
                StopCoroutine(_sequenceCoroutine);
                _sequenceCoroutine = null;
            }
        }

#if UNITY_EDITOR
        [Button]
        public void AddMoveAction(Transform moveTarget)
        {
            _actionSequence.Add(new Move(_actionPerformer, moveTarget.position));
            UnityEditor.EditorUtility.SetDirty(this);
        }

        [Button]
        public void AddAttackAction(ICombatant target, bool forceMiss, GridManager gridManager)
        {
            if (forceMiss)
                _actionSequence.Add(new Attack(target, _actionPerformer.References.CombatBehaviour, new() { HitChanceMod = -200 }, gridManager));
            else
                _actionSequence.Add(new Attack(target, _actionPerformer.References.CombatBehaviour, gridManager));
            UnityEditor.EditorUtility.SetDirty(this);
        }

        [Button]
        public void AddWaitAction(float waitTime)
        {
            _actionSequence.Add(new Wait(waitTime, _actionPerformer));
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}
