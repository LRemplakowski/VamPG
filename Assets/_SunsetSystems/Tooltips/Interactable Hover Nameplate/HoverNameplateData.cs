using System.Collections;
using System.Collections.Generic;
using SunsetSystems.Entities;
using UnityEngine;

namespace SunsetSystems.Tooltips
{
    public class HoverNameplateData : ITooltipContext
    {
        private IHoverNameplateSource _dataSource;

        public GameObject TooltipSource => _dataSource.NameplateSource;
        public Vector3 TooltipPosition => _dataSource.NameplateWorldPosition;
        public string TooltipText => _dataSource.NameplateText;

        public HoverNameplateData(IHoverNameplateSource dataSource)
        {
            _dataSource = dataSource;
        }
    }
}
