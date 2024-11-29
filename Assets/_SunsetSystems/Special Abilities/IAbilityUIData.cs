using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SunsetSystems.Abilities
{
    public interface IAbilityUIData
    {
        Sprite GetAbilityIcon();
        string GetLocalizedName();
        string GetLocalizedDescription();
    }
}
