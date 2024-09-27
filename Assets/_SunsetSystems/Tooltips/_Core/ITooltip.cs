using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Tooltips
{
    public interface ITooltip
    {
        bool InjectTooltipData(ITooltipContext context);
        void RefreshTooltip();
        void UpdateTooltipPosition();
        void SetAlwaysUpdatePosition(bool update);
        void SetParentTransfrom(RectTransform parent);
    }

    public interface ITooltip<T> : ITooltip where T : ITooltipContext
    {
        bool InjectTooltipData(T context);
    }
}
