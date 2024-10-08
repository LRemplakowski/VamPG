using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Core.Localization
{
    public interface ILocalizationTarget
    {
        void SetLocalizedText(string text);
    }
}
