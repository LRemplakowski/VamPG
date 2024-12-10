using Sirenix.OdinInspector;
using Sirenix.Serialization;
using SunsetSystems.Abilities;
using UnityEngine;
using UnityEngine.UI;

namespace SunsetSystems.Combat.UI
{
    public class CombatActionSelectorButton : SerializedMonoBehaviour
    {
        [SerializeField]
        private Image _abilityIcon;
        [SerializeField]
        private CompositeButton _compositeButton;
        [OdinSerialize]
        private SelectedCombatActionData _actionData;

        private CombatUIManager _combatUIManager;

        private void Start()
        {
            _combatUIManager = GetComponentInParent<CombatUIManager>();
        }

        public void SetAbility(IAbility ability)
        {
            _actionData = new(ability);
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

        public void OnClick()
        {
            _combatUIManager?.SelectCombatAction(_actionData);
        }
    }
}
