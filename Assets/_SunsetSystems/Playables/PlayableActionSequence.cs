using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SunsetSystems.Entities.Characters.Actions;
using SunsetSystems.Entities.Interfaces;
using UnityEngine;

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
                _actionPerformer.PerformAction(action, false);
                while (action.ActionFinished is false && action.ActionCanceled is false)
                    yield return null;
            }
            _sequenceCoroutine = null;
            if (_loop)
            {
                _sequenceCoroutine = ActionSequence();
                StartCoroutine(_sequenceCoroutine);
            }
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
        public void AddAttackAction(ICombatant target, bool forceMiss)
        {
            if (forceMiss)
                _actionSequence.Add(new Attack(target, _actionPerformer.References.CombatBehaviour, new() { HitChanceMod = -200 }));
            else
                _actionSequence.Add(new Attack(target, _actionPerformer.References.CombatBehaviour));
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
