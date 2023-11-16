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
        public UltEvent<SelectedCombatActionData> OnCombatActionSelected;

        private List<Selectable> childrenButtons = new();

        [SerializeField]
        private Image currentActorPortrait;
        private AssetReferenceSprite spriteReference;

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

        public async void OnCombatRoundBegin(ICombatant combatant)
        {
            EventSystem.current.SetSelectedGameObject(null);
            childrenButtons.ForEach(button => button.interactable = combatant.Faction is Faction.PlayerControlled);
            if (combatant.Faction == Faction.PlayerControlled)
            {
                await UpdateCurrentActorPortrait(combatant);
            }
        }

        private async Task UpdateCurrentActorPortrait(ICombatant actor)
        {
            AssetReferenceSprite newSpriteRef = actor.GetComponentInChildren<CreatureData>().PortraitAssetRef;
            if (newSpriteRef != null)
            {
                Sprite sprite = await AddressableManager.Instance.LoadAssetAsync(newSpriteRef);
                currentActorPortrait.sprite = sprite;
                if (currentActorPortrait.color == Color.clear)
                    StartCoroutine(ShowPortrait());
                if (spriteReference != null)
                    AddressableManager.Instance.ReleaseAsset(spriteReference);
                spriteReference = newSpriteRef;
            }
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

        public void OnCombatRoundEnd()
        {

        }

        public void SelectCombatAction(SelectedCombatActionData actionData)
        {
            OnCombatActionSelected?.InvokeSafe(actionData);
        }
    }
}
