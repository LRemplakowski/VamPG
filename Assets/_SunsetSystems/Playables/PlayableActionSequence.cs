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
        [SerializeField, ListDrawerSettings(HideAddButton = true), ReadOnly]
        private List<EntityAction> _actionSequence = new();

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
