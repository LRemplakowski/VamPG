using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SunsetSystems.UI
{
    public class HoverNameplate : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI textComponent;

        private void Start()
        {
            textComponent ??= GetComponentInChildren<TextMeshProUGUI>();
            gameObject.SetActive(false);
        }

        public void SetNameplateText(string text)
        {
            textComponent.text = text;
        }
    }
}
