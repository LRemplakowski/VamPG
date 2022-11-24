using SunsetSystems.Combat;
using SunsetSystems.Entities.Characters;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Spellbook
{
    [CreateAssetMenu(fileName = "HealOnEndTurn", menuName = "Scriptable Powers/Heal On End Turn")]
    public class HealOnEndTurn : DisciplineScript
    {
        private List<Creature> _effectRecievers = new();

        private void OnEnable()
        {
            _effectRecievers = new();
            CombatManager.OnFullTurnCompleted += HealOnFullTurn;
        }

        private void OnDisable()
        {
            CombatManager.OnFullTurnCompleted -= HealOnFullTurn;
        }

        public override void Activate(Creature target, Creature caster)
        {
            _effectRecievers ??= new();
            _effectRecievers.RemoveAll(c => c == null);
            _effectRecievers.Add(caster);
        }

        private void HealOnFullTurn()
        {
            _effectRecievers.ForEach(c => DoHealing(c));
        }

        private void DoHealing(Creature c)
        {
            if (c.StatsManager.IsAlive())
            {
                int amount = UnityEngine.Random.Range(1, 3);
                c.StatsManager.Heal(amount);
            }
        }
    }
}
