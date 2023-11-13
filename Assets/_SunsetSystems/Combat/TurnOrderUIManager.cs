using Sirenix.OdinInspector;
using SunsetSystems.Core.AddressableManagement;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Entities.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace SunsetSystems.Combat.UI
{
    public class TurnOrderUIManager : SerializedMonoBehaviour
    {
        [SerializeField]
        private List<Image> turnOrderPortraits = new();
        [SerializeField]
        private List<Animator> animatedArrows = new();
        [SerializeField]
        private string animatedArrowTrigger;
        private int animatedArrowTriggerHash;

        private List<AssetReferenceSprite> portraitReferences = new();
        private IEnumerator animatePortraitArrows;

        private void OnEnable()
        {
            if (animatePortraitArrows != null)
                StopCoroutine(animatePortraitArrows);
            animatePortraitArrows = AnimateArrows();
            StartCoroutine(animatePortraitArrows);
        }

        private void OnDisable()
        {
            if (animatePortraitArrows != null)
                StopCoroutine(animatePortraitArrows);
        }

        private void Start()
        {
            animatedArrowTriggerHash = Animator.StringToHash(animatedArrowTrigger);
            turnOrderPortraits.ForEach(p => p.transform.parent.gameObject.SetActive(false));
        }

        public void OnCombatRoundBegin()
        {
            _ = UpdatePortraits();
        }

        private async Task UpdatePortraits()
        {
            List<ICombatant> combatantsInOrder = CombatManager.Instance.GetCombatantsInTurnOrder();
            List<AssetReferenceSprite> newPortraitReferences = new();
            List<Task<Sprite>> spriteLoadingTasks = new();
            for (int i = 0; i < turnOrderPortraits.Count; i++)
            {
                if (combatantsInOrder.Count <= i)
                {
                    turnOrderPortraits[i].transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    ICombatant combatantAtIndex = combatantsInOrder[i];
                    AssetReferenceSprite portraitRef = combatantAtIndex.References.GetComponentInChildren<CreatureData>().PortraitAssetRef;
                    newPortraitReferences.Add(portraitRef);
                    Task<Sprite> portraitTask = AddressableManager.Instance.LoadAssetAsync(portraitRef);
                    spriteLoadingTasks.Add(portraitTask);
                }
            }
            await Task.WhenAll(spriteLoadingTasks);
            for (int i = 0; i < turnOrderPortraits.Count; i++)
            {
                if (i >= spriteLoadingTasks.Count)
                    break;
                Image image = turnOrderPortraits[i];
                image.sprite = spriteLoadingTasks[i].Result;
                image.transform.parent.gameObject.SetActive(true);
                await Task.Delay(500);
            }
            portraitReferences.ForEach(reference => AddressableManager.Instance.ReleaseAsset(reference));
            portraitReferences = newPortraitReferences;
        }

        private IEnumerator AnimateArrows()
        {
            yield return null;
            for (int i = animatedArrows.Count - 1; i >= 0; i--)
            {
                Animator arrow = animatedArrows[i];
                if (arrow.isActiveAndEnabled)
                {
                    arrow.SetTrigger(animatedArrowTriggerHash);
                    yield return new WaitForSeconds(1f);
                }
                else
                {
                    yield return null;
                }
                if (i <= 0)
                    i = animatedArrows.Count - 1;
            }
        }
    }
}
