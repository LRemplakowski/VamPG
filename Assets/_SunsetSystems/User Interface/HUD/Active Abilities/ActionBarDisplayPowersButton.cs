using SunsetSystems.Combat;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Entities.Interfaces;
using SunsetSystems.Spellbook;
using SunsetSystems.UI.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SunsetSystems.UI
{
    public class ActionBarDisplayPowersButton : MonoBehaviour
    {
        [SerializeField]
        private List<DisciplineType> _powerTypes;
        [SerializeField]
        private ActiveAbilitiesDisplay _powersDisplay;
        [SerializeField]
        private Transform _powersDisplayHookPoint;

        private HashSet<DisciplineType> _hashedPowerTypes;

        private void Awake()
        {
            _hashedPowerTypes = new(_powerTypes.Distinct());
        }

        public void ShowDisciplinePowerDisplay()
        {
            ICombatant activeActor = CombatManager.Instance.CurrentActiveActor;
            if (activeActor == null)
            {
                Debug.LogError("Trying to display power list, but CurrentActiveActor is null!");
                return;
            }
            List<IGameDataProvider<DisciplinePower>> powers = new();
            powers.AddRange(activeActor.References.GetComponentInChildren<StatsManager>().Stats.Disciplines
                .GetDisciplines()
                .FindAll(d => _hashedPowerTypes.Contains(d.GetDisciplineType()))
                // We assume that if given power has a blood cost, it's an active powers. That's a dirty hack, may fail.
                .SelectMany(d => d.GetKnownPowers().FindAll(p => p.BloodCost > 0)));
            _powersDisplay.DisplayAbilities(powers, _powersDisplayHookPoint);
        }
    }
}
