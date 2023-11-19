using Redcode.Awaiting;
using Sirenix.OdinInspector;
using SunsetSystems.Combat;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Entities.Characters.Interfaces;
using SunsetSystems.Entities.Creatures.Interfaces;
using SunsetSystems.Entities.Data;
using SunsetSystems.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace SunsetSystems.Spellbook
{
    /// <summary>
    /// Component to be added to creature to manage individual powers known by the creature.
    /// </summary>
    public class SpellbookManager : SerializedMonoBehaviour, IMagicUser
    {
        [SerializeField]
        private ICreature _owner;
        public ICreatureReferences References => _owner.References;

        private Disciplines Disciplines => throw new NotImplementedException();

        private readonly Dictionary<DisciplinePower, int> _powersOnCooldown = new();

        public static ICombatant PowerTarget { get; set; }
        public static Target RequiredTarget { get; private set; }
        private static Task targetAwaiterTask;
        private CancellationTokenSource targetAwaiterCancellation;

        private void OnEnable()
        {
            CombatManager.Instance.OnFullTurnCompleted += DecreaseCooldowns;
        }

        private void Start()
        {
            ApplyPasivePowers();
        }

        private void OnDisable()
        {
            CombatManager.Instance.OnFullTurnCompleted -= DecreaseCooldowns;
        }

        private async void ApplyPasivePowers()
        {
            await Task.Yield();
            //await new WaitForBackgroundThread();
            //List<DisciplinePower> powers = new();
            //foreach (Discipline discipline in Disciplines.GetDisciplines())
            //{
            //    await new WaitForSeconds(1f);
            //    List<DisciplinePower> passivePowers = discipline.GetKnownPowers()?.FindAll(p => p != null && p.GetEffects().All(e => e.Duration == Duration.Passive));
            //    powers.AddRange(passivePowers);
            //}
            //await new WaitForUpdate();
            //powers.ForEach(p => Spellcaster.HandleEffects(p, _owner));
        }

        public void UsePower(DisciplinePower power, IMagicUser castingActor)
        {
            throw new NotImplementedException();
        }

        public void UsePower(DisciplinePower power, ICombatant target)
        {
            ActionBarUI.Instance.SetBarAction(default);
            if (_powersOnCooldown.ContainsKey(power))
                return;
            if (Disciplines.GetDisciplines().Any(d => d.GetKnownPowers().Contains(power)))
            {
                if (DeducePowerCost(power))
                {
                    Spellcaster.HandleEffects(power, this, target);
                    StartCooldown(power);
                }
            }
            else
            {
                Debug.LogError($"Attempting to use power {power.PowerName} but it is not known by the user!");
            }
        }

        public async void UsePowerAfterTargetSelection(DisciplinePower power)
        {
            RequiredTarget = power.Target;
            await new WaitForBackgroundThread();
            if (targetAwaiterTask != null)
            {
                targetAwaiterCancellation.Cancel();
                targetAwaiterTask = null;
            }
            targetAwaiterCancellation = new();
            targetAwaiterTask = Task.Run(async () =>
            {
                while (PowerTarget == null)
                {
                    await new WaitForSeconds(0.1f);
                }
            }, targetAwaiterCancellation.Token);
            await targetAwaiterTask;
            if (targetAwaiterTask.IsCanceled)
                return;
            targetAwaiterTask = null;
            await new WaitForUpdate();
            UsePower(power, PowerTarget);
        }

        private void StartCooldown(DisciplinePower power)
        {
            _powersOnCooldown.Add(power, power.Cooldown);
        }

        public bool IsPowerOnCooldown(DisciplinePower power)
        {
            return _powersOnCooldown.ContainsKey(power);
        }

        public bool GetIsPowerKnown(DisciplinePower power)
        {
            return Disciplines.GetDisciplines().Any(d => d.GetKnownPowers().Contains(power));
        }

        private void DecreaseCooldowns()
        {
            List<DisciplinePower> powersToRemove = new();
            List<DisciplinePower> powersToDecreaseCooldowns = new();
            foreach (DisciplinePower power in _powersOnCooldown.Keys)
            {
                int cooldown = _powersOnCooldown[power];
                cooldown -= 1;
                if (cooldown <= 0)
                    powersToRemove.Add(power);
                else
                    powersToDecreaseCooldowns.Add(power);
            }
            powersToRemove.ForEach(p => _powersOnCooldown.Remove(p));
            powersToDecreaseCooldowns.ForEach(p => _powersOnCooldown[p] -= 1);
        }

        private bool DeducePowerCost(DisciplinePower power)
        {
            if (_owner.Faction is Faction.PlayerControlled)
                throw new NotImplementedException();
            else
                return true;
        }
    }
}
