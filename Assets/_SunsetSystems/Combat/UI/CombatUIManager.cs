using System.Collections;
using Sirenix.OdinInspector;
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

        [Title("Events")]
        public UltEvent<SelectedCombatActionData> OnCombatActionSelected;

        private void OnEnable()
        {
            currentActorPortrait.color = Color.clear;
        }

        public void OnCombatBegin()
        {
            _combatCanvasGroup.interactable = false;
            _combatCanvasGroup.blocksRaycasts = true;
            _combatCanvasGroup.alpha = 1f;
        }

        public void OnCombatEnd()
        {
            _combatCanvasGroup.interactable = false;
            _combatCanvasGroup.blocksRaycasts = false;
            _combatCanvasGroup.alpha = 0f;
        }

        public void OnCombatRoundBegin(ICombatant combatant)
        {
            EventSystem.current.SetSelectedGameObject(null);
            _combatCanvasGroup.interactable = combatant.Faction is Faction.PlayerControlled;
            if (combatant.Faction == Faction.PlayerControlled)
            {
                UpdateCurrentActorPortrait(combatant);
                currentActorHealth.UpdateHealthDisplay(combatant);
                apBar.UpdateActiveChunks((combatant.HasActed ? 0 : 1) + (combatant.HasMoved ? 0 : 1));
                bpBar.UpdateActiveChunks(combatant.References.StatsManager.Hunger.GetValue());
                disciplineBar.ShowAbilities(combatant);
                _actionBarUI.RefreshAvailableActions();
                combatant.OnUsedActionPoint += OnActionUsed;
                combatant.OnSpentBloodPoint += OnBloodPointSpent;
                var weaponManager = combatant.WeaponManager;
                _ammoCounter.UpdateAmmoData(weaponManager.GetSelectedWeaponAmmoData());
                _ammoCounter.SetAmmoCounterVisible(weaponManager.GetSelectedWeapon().GetWeaponUsesAmmo());
                weaponManager.OnWeaponChanged += OnWeaponChanged;
                weaponManager.OnAmmoChanged += OnAmmoChanged;
            }
        }

        private void OnWeaponChanged(ICombatant combatant)
        {
            _actionBarUI.RefreshAvailableActions();
            var weaponManager = combatant.References.WeaponManager;
            _ammoCounter.UpdateAmmoData(weaponManager.GetSelectedWeaponAmmoData());
            _ammoCounter.SetAmmoCounterVisible(weaponManager.GetSelectedWeapon().GetWeaponUsesAmmo());
        }

        private void OnActionUsed(ICombatant combatant)
        {
            apBar.UpdateActiveChunks((combatant.HasActed ? 0 : 1) + (combatant.HasMoved ? 0 : 1));
        }

        private void OnBloodPointSpent(ICombatant combatant)
        {
            bpBar.UpdateActiveChunks(combatant.References.StatsManager.Hunger.GetValue());
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
            combatant.OnUsedActionPoint -= OnActionUsed;
            combatant.OnSpentBloodPoint -= OnBloodPointSpent;
            var weaponManager = combatant.WeaponManager;
            weaponManager.OnWeaponChanged -= OnWeaponChanged;
            weaponManager.OnAmmoChanged -= OnAmmoChanged;
        }

        public void SelectCombatAction(SelectedCombatActionData actionData)
        {
            OnCombatActionSelected?.InvokeSafe(actionData);
        }
    }
}
