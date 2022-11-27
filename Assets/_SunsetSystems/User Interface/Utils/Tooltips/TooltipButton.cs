using Redcode.Awaiting;
using SunsetSystems.UI.Utils;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SunsetSystems.UI
{
    [System.Serializable]
    public class TooltipButton : Button
    {
        [SerializeField]
        private GameTooltip _tooltip;
        [SerializeField]
        private Transform _tooltipParent;
        private TooltipContent _content;
        [SerializeField, Range(0, 10)]
        private float _tooltipDelay;

        private bool _waitingForTooltip;

        public void SetContent(TooltipContent content)
        {
            _content = content;
        }

        public void Initialize(GameTooltip tooltipInstance)
        {
            _tooltip = tooltipInstance;
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            _waitingForTooltip = true;
            if (_tooltipDelay > 0f)
                _ = DisplayTooltipAfterDelay();
            else
                _tooltip.ShowTooltip(_tooltipParent, _content);
        }

        private async Task DisplayTooltipAfterDelay()
        {
            float elapsedTime = 0f;
            while (elapsedTime < _tooltipDelay && _waitingForTooltip)
            {
                elapsedTime += Time.deltaTime;
                await new WaitForUpdate();
            }
            if (_waitingForTooltip)
                _tooltip.ShowTooltip(_tooltipParent, _content);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            _waitingForTooltip = false;
            _tooltip.HideTooltip();
        }
    }
}
