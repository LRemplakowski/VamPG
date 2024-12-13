using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using SunsetSystems.Abilities;
using UnityEngine;
using UnityEngine.UI;

namespace SunsetSystems.Combat.UI
{
    public class CombatActionSelectorButton : SerializedMonoBehaviour, IAbilityButton
    {
        [Title("Config")]
        [SerializeField]
        private Image _abilityIcon;
        [SerializeField]
        private CompositeButton _compositeButton;
        [Title("Runtime")]
        [ShowInInspector, ReadOnly]
        private IAbilityConfig _cachedAbility;
        private Action<IAbilityConfig> _selectionDelegate;

        public void OnClick()
        {
            _selectionDelegate?.Invoke(_cachedAbility);
        }

        public void Initialize(IAbilityConfig ability, Action<IAbilityConfig> selectionDelegate)
        {
            CacheAbility(ability);
            CacheSelectionDelegate(selectionDelegate);
            SetupButtonVisuals(ability);
        }

        private void CacheAbility(IAbilityConfig ability) => _cachedAbility = ability;
        private void CacheSelectionDelegate(Action<IAbilityConfig> selectionDelegate) => _selectionDelegate = selectionDelegate;

        private void SetupButtonVisuals(IAbilityConfig ability)
        {
            var abilituGUIdata = ability.GetAbilityUIData();
            _abilityIcon.sprite = abilituGUIdata.GetAbilityIcon(IAbilityUIData.IconState.Default);
            _compositeButton.ClearCompositeData();
            _compositeButton.AddSpriteSwapComposite(new()
            {
                Image = _abilityIcon,
                HighlightedSprite = abilituGUIdata.GetAbilityIcon(IAbilityUIData.IconState.Highlighted),
                PressedSprite = abilituGUIdata.GetAbilityIcon(IAbilityUIData.IconState.Pressed),
                SelectedSprite = abilituGUIdata.GetAbilityIcon(IAbilityUIData.IconState.Selected),
                DisabledSprite = abilituGUIdata.GetAbilityIcon(IAbilityUIData.IconState.Disabled)
            });
        }
    }
}
