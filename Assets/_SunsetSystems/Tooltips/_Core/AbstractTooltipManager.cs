using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using SunsetSystems.Utils.ObjectPooling;
using UnityEngine;

namespace SunsetSystems.Tooltips
{
    public abstract class AbstractTooltipManager<T, U> : SerializedMonoBehaviour where T : ITooltipContext where U : AbstractTooltip<T>
    {
        [TabGroup("Tooltip Manager")]
        [Title("Config")]
        [SerializeField, LabelWidth(300)]
        protected bool _alwaysUpdateTooltipPosition = false;
        [TabGroup("Tooltip Manager")]
        [SerializeField, LabelWidth(300)]
        protected bool _convertPositionToCanvasSpace = true;
        [TabGroup("Tooltip Manager")]
        [Title("References")]
        [SerializeField]
        private IObjectPool<U> _tooltipPool;
        [TabGroup("Tooltip Manager")]
        [SerializeField]
        private RectTransform _tooltipParentRect;

        [TabGroup("Tooltip Manager")]
        [Title("Runtime")]
        [ShowInInspector, ReadOnly]
        private Dictionary<GameObject, U> _activeTooltips = new();

        public void HideTooltip(ITooltipContext tooltipContext)
        {
            if (_activeTooltips.TryGetValue(tooltipContext.TooltipSource, out var tooltipInstance))
            {
                Debug.Log($"{gameObject.name} >>> Removing tooltip from source: {tooltipContext.TooltipSource}");
                _tooltipPool.ReturnObject(tooltipInstance);
                _activeTooltips.Remove(tooltipContext.TooltipSource);
            }
        }

        public U ShowTooltip(ITooltipContext tooltipContext)
        {
            if (_activeTooltips.TryGetValue(tooltipContext.TooltipSource, out U tooltip))
            {
                if (tooltip.InjectTooltipData(tooltipContext))
                {
                    Debug.Log($"{gameObject.name} >>> Updating tooltip from source: {tooltipContext.TooltipSource}");
                    tooltip.SetParentTransfrom(_tooltipParentRect);
                    tooltip.SetConvertPositionToCanvasSpace(_convertPositionToCanvasSpace);
                    tooltip.UpdateTooltipPosition();
                    tooltip.SetAlwaysUpdatePosition(_alwaysUpdateTooltipPosition);
                }
                else
                {
                    Debug.LogWarning($"{gameObject.name} >>> Failed to update tooltip from source: {tooltipContext.TooltipSource}");
                }
            }
            else
            {

                tooltip = _tooltipPool.GetPooledObject();
                if (tooltip.InjectTooltipData(tooltipContext))
                {
                    Debug.Log($"{gameObject.name} >>> Creating new tooltip from source: {tooltipContext.TooltipSource}");
                    tooltip.SetParentTransfrom(_tooltipParentRect);
                    tooltip.SetConvertPositionToCanvasSpace(_convertPositionToCanvasSpace);
                    tooltip.UpdateTooltipPosition();
                    tooltip.SetAlwaysUpdatePosition(_alwaysUpdateTooltipPosition);
                    _activeTooltips[tooltipContext.TooltipSource] = tooltip;
                }
                else
                {
                    Debug.LogError($"{gameObject.name} >>> Failed to create from source: {tooltipContext.TooltipSource}");
                    _tooltipPool.ReturnObject(tooltip);
                    tooltip = default;
                }
            }
            return tooltip;
        }

        public void UpdateAllTooltipPositions()
        {
            _activeTooltips.Values.ForEach(tooltip => tooltip.UpdateTooltipPosition());
        }

        public void UpdateTooltipPosition(ITooltipContext tooltipContext)
        {
            if (_activeTooltips.TryGetValue(tooltipContext.TooltipSource, out var tooltip))
                tooltip.UpdateTooltipPosition();
        }

        public void RefreshTooltip(GameObject tooltipSource)
        {
            if (_activeTooltips.TryGetValue(tooltipSource, out var tooltip))
                tooltip.RefreshTooltip();
        }

        public void RefreshAllTooltips()
        {
            foreach (var tooltip in _activeTooltips.Values)
                tooltip.RefreshTooltip();
        }
    }
}
