using Sirenix.OdinInspector;
using SunsetSystems.Combat;
using UnityEngine;

namespace SunsetSystems.AI
{
    public class AIBehaviourContext : SerializedMonoBehaviour
    {
        [SerializeField]
        private ICombatant _combatBehaviour;

        private CombatManager _combatManager;

        private void Start()
        {
            _combatManager = CombatManager.Instance;
        }

        public bool IsMyTurn()
        {
            return _combatManager.IsCurrentActiveActor(_combatBehaviour);
        }

        public bool CanMove()
        {
            return _combatBehaviour.HasActionsQueued is false && _combatBehaviour.GetCanMove();
        }
    }
}
