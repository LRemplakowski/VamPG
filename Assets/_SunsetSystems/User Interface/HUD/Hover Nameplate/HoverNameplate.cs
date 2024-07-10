using System.Collections;
using System.Collections.Generic;
using SunsetSystems.Utils.ObjectPooling;
using TMPro;
using UnityEngine;

namespace SunsetSystems.UI
{
    public class HoverNameplate : MonoBehaviour, IPooledObject
    {
        [field: SerializeField]
        public RectTransform Rect { get; private set; }

        [SerializeField]
        private TextMeshProUGUI _textComponent;

        private void Start()
        {
            if (Rect == null)
                Rect = transform as RectTransform;
            _textComponent ??= GetComponentInChildren<TextMeshProUGUI>();
        }

        public void SetNameplateText(string text)
        {
            _textComponent.text = text;
        }

        public void ResetObject()
        {
            _textComponent.text = "";
        }
    }
}
