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
        [field: SerializeField]
        private List<Discipline> Disciplines { get; set; }

        public IEnumerable<DisciplinePower> KnownPowers => Disciplines.SelectMany(d => d.GetKnownPowers());

        private readonly Dictionary<DisciplinePower, int> _powersOnCooldown = new();

        public static ICombatant PowerTarget { get; set; }
        public static Target RequiredTarget { get; private set; }
        private static Task targetAwaiterTask;
        private CancellationTokenSource targetAwaiterCancellation;

        private void Reset()
        {
            _owner = GetComponentInParent<ICreature>();
            Disciplines = new();
            foreach (DisciplineType disciplineType in Enum.GetValues(typeof(DisciplineType)))
            {
                Disciplines.Add(new Discipline(disciplineType));
            }
        }

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

        private void ApplyPasivePowers()
        {
            List<DisciplinePower> powers = new();
            List<DisciplinePower> passivePowers = Disciplines.SelectMany(d => d.GetKnownPowers()).ToList().FindAll(p => p != null && p.GetEffects().All(e => e.Duration == Duration.Passive));
            powers.AddRange(passivePowers);
            powers.ForEach(p => Spellcaster.HandleEffects(p, _owner.References.CombatBehaviour.MagicUser));
        }

        public void UsePower(DisciplinePower power, IMagicUser castingActor)
        {
            UsePower(power, castingActor.References.CombatBehaviour);
        }

        public void UsePower(DisciplinePower power, ICombatant target)
        {
            if (_powersOnCooldown.ContainsKey(power))
                return;
            if (GetIsPowerKnown(power) && DeducePowerCost(power))
            {
                Spellcaster.HandleEffects(power, this, target);
                StartCooldown(power);
            }
            else
            {
                Debug.LogError($"Attempting to use power {power.PowerName} but it is not known by the user!");
            }
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
            return Disciplines.Any(d => d.GetKnownPowers().Contains(power));
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
            return true;
            //if (_owner.Faction is Faction.PlayerControlled)
            //    throw new NotImplementedException();
            //else
            //    return true;
        }
    }
}
