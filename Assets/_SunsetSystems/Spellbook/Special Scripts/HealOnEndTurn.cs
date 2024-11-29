using SunsetSystems.Combat;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Entities.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Abilities
{
    [CreateAssetMenu(fileName = "HealOnEndTurn", menuName = "Scriptable Powers/Heal On End Turn")]
    public class HealOnEndTurn : DisciplineScript
    {
        private List<ICombatant> _effectRecievers = new();

        private void Awake()
        {
            _effectRecievers = new();
            
            //if (CombatManager.Instance != null)
                //CombatManager.Instance.OnFullTurnCompleted += HealOnFullTurn;
        }

        private void OnDestroy()
        {
            //if (CombatManager.Instance != null)
                //CombatManager.Instance.OnFullTurnCompleted -= HealOnFullTurn;
        }

        public override void Activate(ICombatant target, ICombatant caster)
        {
            _effectRecievers ??= new();
            _effectRecievers.RemoveAll(c => c == null);
            _effectRecievers.Add(caster);
        }

        private void HealOnFullTurn()
        {
            _effectRecievers.ForEach(c => DoHealing(c));
        }

        private void DoHealing(ICombatant creature)
        {
            StatsManager targetStatsManager = creature.References.GetCachedComponentInChildren<StatsManager>();
            if (targetStatsManager != null && targetStatsManager.IsAlive())
            {
                int amount = UnityEngine.Random.Range(1, 3);
                targetStatsManager.Heal(amount);
            }
        }
    }
}
