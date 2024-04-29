using Sirenix.OdinInspector;
using SunsetSystems.Core.AddressableManagement;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Entities.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UltEvents;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SunsetSystems.Combat.UI
{
    public class CombatUIManager : SerializedMonoBehaviour
    {
        [Title("References")]
        [SerializeField]
        private Image currentActorPortrait;
        [SerializeField]
        private PlayerHealthDisplay currentActorHealth;
        [SerializeField]
        private ResourceBarDisplay apBar, bpBar;
        [SerializeField]
        private ActiveAbilitiesDisplayManager disciplineBar;

        [Title("Events")]
        public UltEvent<SelectedCombatActionData> OnCombatActionSelected;

        private List<Selectable> childrenButtons = new();

        private void OnEnable()
        {
            currentActorPortrait.color = Color.clear;
        }

        private void Start()
        {
            childrenButtons = GetComponentsInChildren<Selectable>(true).ToList();
        }

        public void OnCombatBegin()
        {
            childrenButtons.ForEach(b => b.interactable = false);
        }

        public void OnCombatRoundBegin(ICombatant combatant)
        {
            EventSystem.current.SetSelectedGameObject(null);
            childrenButtons.ForEach(button => button.interactable = combatant.Faction is Faction.PlayerControlled);
            if (combatant.Faction == Faction.PlayerControlled)
            {
                UpdateCurrentActorPortrait(combatant);
                currentActorHealth.UpdateHealthDisplay();
                apBar.UpdateActiveChunks((combatant.HasActed ? 0 : 1) + (combatant.HasMoved ? 0 : 1));
                bpBar.UpdateActiveChunks(combatant.References.StatsManager.Hunger.GetValue());
                disciplineBar.ShowAbilities(combatant);
                combatant.OnUsedActionPoint += OnActionUsed;
                combatant.OnSpentBloodPoint += OnBloodPointSpent;
            }
        }

        private void OnActionUsed(ICombatant combatant)
        {
            apBar.UpdateActiveChunks((combatant.HasActed ? 0 : 1) + (combatant.HasMoved ? 0 : 1));
        }

        private void OnBloodPointSpent(ICombatant combatant)
        {
            bpBar.UpdateActiveChunks(combatant.References.StatsManager.Hunger.GetValue());
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
        }

        public void SelectCombatAction(SelectedCombatActionData actionData)
        {
            OnCombatActionSelected?.InvokeSafe(actionData);
        }
    }
}
