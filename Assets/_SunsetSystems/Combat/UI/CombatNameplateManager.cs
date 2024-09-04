using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SunsetSystems.Entities.Interfaces;
using SunsetSystems.Utils.ObjectPooling;
using UnityEngine;

namespace SunsetSystems.Combat.UI
{
    public class CombatNameplateManager : AbstractObjectPool<CombatNameplate>
    {
        [Title("References")]
        [SerializeField, Required]
        private RectTransform _nameplateParentTransform;
        [Title("Runtime")]
        [ShowInInspector, ReadOnly]
        private Dictionary<ICombatant, CombatNameplate> _nameplateMap = new();

        protected override void Awake()
        {
            base.Awake();
        }

        private void Update()
        {
            foreach (var kvpair in _nameplateMap)
            {
                UpdateNameplatePosition(kvpair.Value, kvpair.Key.NameplatePosition);
            }
        }

        public void OnCombatBegin(IEnumerable<ICombatant> combatants)
        {
            foreach (var combatant in combatants)
            {
                var nameplate = GetPooledObject();
                nameplate.UpdateNameplateData(new(combatant));
                _nameplateMap[combatant] = nameplate;
                combatant.WeaponManager.OnWeaponChanged += OnCombatantUpdate;
                combatant.OnDamageTaken += OnCombatantUpdate;
                UpdateNameplatePosition(nameplate, combatant.NameplatePosition);
            }
        }

        private void OnCombatantUpdate(ICombatant combatant)
        {
            if (_nameplateMap.TryGetValue(combatant, out var nameplateInstance))
                nameplateInstance.UpdateNameplateData(new(combatant));
        }

        private void UpdateNameplatePosition(CombatNameplate nameplate, Vector3 worldPosition)
        {
            var screenPoint = Camera.main.WorldToScreenPoint(worldPosition);
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_nameplateParentTransform, screenPoint, null, out Vector2 localPoint))
            {
                nameplate.NameplateRect.localPosition = localPoint;
            }
        }

        public void OnCombatEnd()
        {
            foreach (var kvpair in _nameplateMap)
            {
                var combatant = kvpair.Key;
                combatant.WeaponManager.OnWeaponChanged -= OnCombatantUpdate;
                combatant.OnDamageTaken -= OnCombatantUpdate;
                ReturnObject(kvpair.Value);
            }
            _nameplateMap.Clear();
        }
    }
}
