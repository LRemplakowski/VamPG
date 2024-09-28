using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace SunsetSystems.Tooltips
{
    public class HoverNameplate : AbstractTooltip<HoverNameplateData>
    {
        [Title("References")]
        [SerializeField]
        private TextMeshProUGUI _textComponent;

        private void Start()
        {
            _textComponent ??= GetComponentInChildren<TextMeshProUGUI>();
        }

        private void SetNameplateText(string text)
        {
            _textComponent.text = text;
        }

        protected override void UpdateTooltipFromContext(HoverNameplateData context)
        {
            SetNameplateText(context.TooltipText);
        }

        protected override void DoCleanUp()
        {
            SetNameplateText("");
        }
    }
}
