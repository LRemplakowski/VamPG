using Redcode.Awaiting;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Entities.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using static SunsetSystems.Spellbook.DisciplinePower.EffectWrapper;

namespace SunsetSystems.Spellbook
{
    /// <summary>
    /// Component to be added to creature to manage individual powers known by the creature.
    /// </summary>
    public class SpellbookManager : MonoBehaviour
    {
        [SerializeField]
        private Creature _owner;
        private ref Disciplines Disciplines => ref _owner.Data.Stats.Disciplines;

        public void Initialize(Creature owner)
        {
            _owner = owner;
            ApplyPasivePowers();
        }

        private async void ApplyPasivePowers()
        {
            await new WaitForBackgroundThread();
            List<DisciplinePower> powers = new();
            foreach (Discipline discipline in Disciplines.GetDisciplines())
            {
                await new WaitForSeconds(1f);
                List<DisciplinePower> passivePowers = discipline.GetKnownPowers().FindAll(p => p != null && p.GetEffects().All(e => e.Duration == Duration.Passive));
                powers.AddRange(passivePowers);
            }
            await new WaitForUpdate();
            powers.ForEach(p => Spellcaster.HandleEffects(p, _owner));
        }

        public void UsePower(DisciplinePower power, Creature target)
        {
            if (Disciplines.GetDisciplines().Any(d => d.GetKnownPowers().Contains(power)))
            {
                Spellcaster.HandleEffects(power, _owner, target);
                DeducePowerCost(power);
                StartCooldown(power);
            }
            else
            {
                Debug.LogError($"Attempting to use power {power.PowerName} but it is not known by the user!");
            }
        }

        private void StartCooldown(DisciplinePower power)
        {

        }

        private void DeducePowerCost(DisciplinePower power)
        {

        }
    }
}
