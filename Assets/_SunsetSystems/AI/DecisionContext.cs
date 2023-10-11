using Sirenix.OdinInspector;
using SunsetSystems.Combat;
using SunsetSystems.Combat.Grid;
using SunsetSystems.Entities.Characters.Actions;
using SunsetSystems.Entities.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.AI
{
    public class DecisionContext : SerializedMonoBehaviour, IDecisionContext
    {
        [field: SerializeField, Required]
        public ICombatant Owner { get; private set; }
        [field: SerializeField, Required]
        public IActionPerformer ActionPerformer { get; private set; }

        public CachedMultiLevelGrid CurrentGrid => CombatManager.Instance.CurrentEncounter.MyGrid;

        public IEnumerable<ICombatant> FriendlyCombatants => CombatManager.Instance.Actors.FindAll(actor => actor.Faction is Faction.Friendly || actor.Faction is Faction.PlayerControlled);

        public IEnumerable<ICombatant> HostileCombatants => CombatManager.Instance.Actors.FindAll(actor => actor.Faction is Faction.Hostile);

        public bool CanMove => Owner.HasMoved is false;

        public bool CanAct => Owner.HasActed is false;

        public bool IsMyTurn => CombatManager.Instance.CurrentActiveActor.Equals(Owner);
    }
}
