using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Tooltips
{
    public interface ITooltipContext
    {
        GameObject TooltipSource { get; }
        Vector3 TooltipPosition { get; }
    }
}
