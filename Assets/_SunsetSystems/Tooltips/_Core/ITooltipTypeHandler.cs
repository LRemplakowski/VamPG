using System.Collections.Generic;

namespace SunsetSystems.Tooltips
{
    public interface ITooltipTypeHandler
    {
        void HideTooltip(ITooltipContext tooltipContext);
        void ShowTooltip(ITooltipContext tooltipContext);
        void UpdateTooltipPosition(ITooltipContext tooltipContext);
        void ShowMultipleTooltips(IList<ITooltipContext> tooltipContexts);
        void HideMultipleTooltips(IList<ITooltipContext> tooltipContexts);
        void UpdateAllTooltipPositions();
    }
}
