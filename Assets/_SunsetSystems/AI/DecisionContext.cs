using Sirenix.OdinInspector;
using SunsetSystems.Combat;
using SunsetSystems.Combat.Grid;
using SunsetSystems.Entities.Characters.Actions;
using SunsetSystems.Entities.Interfaces;
using SunsetSystems.Equipment;
using SunsetSystems.Inventory;
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

        public GridManager GridManager => CombatManager.Instance.CurrentEncounter.GridManager;

        public List<ICombatant> FriendlyCombatants => CombatManager.Instance.Actors.FindAll(actor => actor.Faction is Faction.Friendly || actor.Faction is Faction.PlayerControlled);

        public List<ICombatant> HostileCombatants => CombatManager.Instance.Actors.FindAll(actor => actor.Faction is Faction.Hostile);

        public bool CanMove => Owner.HasMoved is false;

        public bool CanAct => Owner.HasActed is false;

        public bool IsMyTurn => CombatManager.Instance.CurrentActiveActor?.Equals(Owner) ?? false;

        public WeaponType CurrentWeaponType => Owner.References.GetCachedComponentInChildren<IWeaponManager>().GetSelectedWeapon().WeaponType;
    }
}
