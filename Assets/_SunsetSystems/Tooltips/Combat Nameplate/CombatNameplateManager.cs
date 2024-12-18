using System.Collections.Generic;
using Sirenix.OdinInspector;
using SunsetSystems.Combat;

namespace SunsetSystems.Tooltips
{
    public class CombatNameplateManager : AbstractTooltipManager<CombatNameplateData, CombatNameplate>
    {
        [TabGroup("Combat Nameplate Manager")]
        [Title("Runtime")]
        [ShowInInspector, ReadOnly]
        private Dictionary<ICombatant, ITooltipContext> _nameplateMap = new();

        private void Awake()
        {
            CombatManager.OnCombatStart += OnCombatBegin;
            CombatManager.OnCombatEnd += OnCombatEnd;
        }

        private void OnDestroy()
        {
            CombatManager.OnCombatStart -= OnCombatBegin;
            CombatManager.OnCombatEnd -= OnCombatEnd;
        }

        public void OnCombatBegin(IEnumerable<ICombatant> combatants)
        {
            foreach (var combatant in combatants)
            {
                var weaponManager = combatant.GetContext().WeaponManager;
                var tooltipContext = new CombatNameplateData(combatant);
                var nameplate = ShowTooltip(tooltipContext);
                _nameplateMap[combatant] = tooltipContext;
                weaponManager.OnWeaponChanged += OnCombatantUpdate;
                combatant.OnDamageTaken += OnCombatantUpdate;
            }
        }

        private void OnCombatantUpdate(ICombatant combatant)
        {
            if (_nameplateMap.TryGetValue(combatant, out var nameplateInstance))
                RefreshTooltip(nameplateInstance.TooltipSource);
        }

        public void OnCombatEnd(IEnumerable<ICombatant> _)
        {
            foreach (var kvpair in _nameplateMap)
            {
                var combatant = kvpair.Key;
                var weaponManager = combatant.GetContext().WeaponManager;
                weaponManager.OnWeaponChanged -= OnCombatantUpdate;
                combatant.OnDamageTaken -= OnCombatantUpdate;
                HideTooltip(kvpair.Value);
            }
            _nameplateMap.Clear();
        }
    }
}
