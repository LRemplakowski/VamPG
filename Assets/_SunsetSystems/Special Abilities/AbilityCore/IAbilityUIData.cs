using UnityEngine;

namespace SunsetSystems.Abilities
{
    public interface IAbilityUIData
    {
        Sprite GetAbilityIcon(IconState iconState);
        string GetLocalizedName();
        string GetLocalizedDescription();

        public enum IconState
        {
            Default, Highlighted, Pressed, Selected, Disabled
        }
    }
}
