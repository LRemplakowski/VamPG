using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SunsetSystems.Core;

namespace UI.CharacterPortraits
{
    public class HungerTrackerUI : MonoBehaviour
    {
        [SerializeField]
        private Sprite hungerEmpty, hungerFull;
        List<Image> hungerSprites = new List<Image>();
        [SerializeField]
        private GameObject hungerDotPrefab;

        private void Start()
        {
            foreach (Transform t in transform.GetComponentsInChildren<Transform>())
            {
                if (!t.gameObject.Equals(this.gameObject))
                    Destroy(t.gameObject);
            }

            for (int i = 0; i < GameConstants.MAX_HUNGER; i++)
            {
                GameObject dotGO = Instantiate(hungerDotPrefab, this.transform);
                dotGO.name = "hungerDot" + i;
                Image image = dotGO.GetComponent<Image>();
                hungerSprites.Add(image);
            }
        }

        internal void SetCurrentHunger(int hunger)
        {
            hunger = Mathf.Clamp(hunger, GameConstants.MIN_HUNGER, GameConstants.MAX_HUNGER);

            foreach (Image hungerDot in hungerSprites)
            {
                hungerDot.sprite = hungerEmpty;
            }

            for (int i = 0; i < hunger; i++)
            {
                hungerSprites[i].sprite = hungerFull;
            }
        }
    }
}
