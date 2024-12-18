using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SunsetSystems.Abilities;
using SunsetSystems.Entities.Interfaces;
using SunsetSystems.Equipment;
using UltEvents;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SunsetSystems.Combat.UI
{
    public class CombatUIManager : SerializedMonoBehaviour
    {
        [Title("References")]
        [SerializeField, Required]
        private Image currentActorPortrait;
        [SerializeField, Required]
        private ActionBarUI _actionBarUI;
        [SerializeField, Required]
        private PlayerHealthDisplay currentActorHealth;
        [SerializeField, Required]
        private ResourceBarDisplay apBar, bpBar;
        [SerializeField, Required]
        private ActiveAbilitiesDisplayManager disciplineBar;
        [SerializeField, Required]
        private CanvasGroup _combatCanvasGroup;
        [SerializeField, Required]
        private AmmoDisplay _ammoCounter;

        private void Awake()
        {
            CombatManager.OnCombatStart += OnCombatBegin;
            CombatManager.OnCombatEnd += OnCombatEnd;
            CombatManager.OnCombatRoundBegin += OnCombatRoundBegin;
            CombatManager.OnCombatRoundEnd += OnCombatRoundEnd;
        }

        private void OnEnable()
        {
            currentActorPortrait.color = Color.clear;
        }

        private void OnDestroy()
        {
            CombatManager.OnCombatStart -= OnCombatBegin;
            CombatManager.OnCombatEnd -= OnCombatEnd;
            CombatManager.OnCombatRoundBegin -= OnCombatRoundBegin;
            CombatManager.OnCombatRoundEnd -= OnCombatRoundEnd;
        }

        public void OnCombatBegin(IEnumerable<ICombatant> _)
        {
            _combatCanvasGroup.interactable = false;
            _combatCanvasGroup.blocksRaycasts = true;
            _combatCanvasGroup.alpha = 1f;
        }

        public void OnCombatEnd(IEnumerable<ICombatant> _)
        {
            _combatCanvasGroup.interactable = false;
            _combatCanvasGroup.blocksRaycasts = false;
            _combatCanvasGroup.alpha = 0f;
        }

        public void OnCombatRoundBegin(ICombatant combatant)
        {
            EventSystem.current.SetSelectedGameObject(null);
            _combatCanvasGroup.interactable = combatant.GetContext().IsPlayerControlled;
            if (combatant.GetContext().IsPlayerControlled)
            {
                UpdateCurrentActorPortrait(combatant);
                currentActorHealth.UpdateHealthDisplay(combatant);
                apBar.UpdateActiveChunks(combatant.GetContext().ActionPointManager.GetCurrentActionPoints());
                bpBar.UpdateActiveChunks(combatant.GetContext().BloodPointManager.GetCurrentBloodPoints());
                disciplineBar.ShowAbilities(combatant);
                _actionBarUI.RefreshAvailableActions();
                combatant.OnUsedActionPoint += OnActionUsed;
                combatant.OnSpentBloodPoint += OnBloodPointSpent;
                var weaponManager = combatant.GetContext().WeaponManager;
                _ammoCounter.UpdateAmmoData(weaponManager.GetSelectedWeaponAmmoData());
                _ammoCounter.SetAmmoCounterVisible(weaponManager.GetSelectedWeapon().GetWeaponUsesAmmo());
                weaponManager.OnWeaponChanged += OnWeaponChanged;
                weaponManager.OnAmmoChanged += OnAmmoChanged;
            }
        }

        private void OnWeaponChanged(ICombatant combatant)
        {
            _actionBarUI.RefreshAvailableActions();
            var weaponManager = combatant.GetContext().WeaponManager;
            _ammoCounter.UpdateAmmoData(weaponManager.GetSelectedWeaponAmmoData());
            _ammoCounter.SetAmmoCounterVisible(weaponManager.GetSelectedWeapon().GetWeaponUsesAmmo());
        }

        private void OnActionUsed(ICombatant combatant)
        {
            apBar.UpdateActiveChunks(combatant.GetContext().ActionPointManager.GetCurrentActionPoints());
        }

        private void OnBloodPointSpent(ICombatant combatant)
        {
            bpBar.UpdateActiveChunks(combatant.GetContext().BloodPointManager.GetCurrentBloodPoints());
        }

        private void OnAmmoChanged(ICombatant combatant, WeaponAmmoData ammoData)
        {
            _ammoCounter.UpdateAmmoData(ammoData);
        }

        private void UpdateCurrentActorPortrait(ICombatant actor)
        {
            currentActorPortrait.sprite = actor.References.CreatureData.Portrait;
            if (currentActorPortrait.color == Color.clear)
                StartCoroutine(ShowPortrait());
        }

        private IEnumerator ShowPortrait()
        {
            float time = 0f;
            while (time < 1f)
            {
                currentActorPortrait.color = Color.Lerp(Color.clear, Color.white, time);
                time += Time.deltaTime;
                yield return null;
            }
        }

        public void OnCombatRoundEnd(ICombatant combatant)
        {
            var context = combatant.GetContext();
            
            combatant.OnUsedActionPoint -= OnActionUsed;
            combatant.OnSpentBloodPoint -= OnBloodPointSpent;
            var weaponManager = combatant.GetContext().WeaponManager;
            weaponManager.OnWeaponChanged -= OnWeaponChanged;
            weaponManager.OnAmmoChanged -= OnAmmoChanged;
        }
    }
}
