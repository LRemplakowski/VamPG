using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using SunsetSystems.Entities.Interfaces;
using UnityEngine;
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

        private IEnumerator animatePortraitArrows;

        private bool firstIteration = false;

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

        public async void OnCombatRoundBegin()
        {
            await UpdatePortraits();
        }

        public async void OnCombatTurnEnd()
        {
            await UpdatePortraits();
        }

        private async Task UpdatePortraits()
        {
            List<ICombatant> combatantsInOrder = CombatManager.Instance.GetCombatantsInTurnOrder();
            for (int i = 0; i < turnOrderPortraits.Count; i++)
            {
                if (combatantsInOrder.Count <= i)
                    turnOrderPortraits[i].transform.parent.gameObject.SetActive(false);
            }
            for (int i = 0; i < turnOrderPortraits.Count; i++)
            {
                if (combatantsInOrder.Count <= i)
                {
                    turnOrderPortraits[i].transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    Image image = turnOrderPortraits[i];
                    image.sprite = combatantsInOrder[i].References.CreatureData.Portrait;
                    image.transform.parent.gameObject.SetActive(true);
                    if (!firstIteration)
                        await Task.Delay(500);
                }
            }
            firstIteration = true;
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
                    i = animatedArrows.Count;
            }
        }
    }
}
