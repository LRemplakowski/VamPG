using Sirenix.OdinInspector;
using SunsetSystems.Combat;
using SunsetSystems.Spellbook;
using SunsetSystems.UI.Utils;
using System;
using TMPro;
using UnityEngine;

namespace SunsetSystems.UI
{
    [RequireComponent(typeof(TooltipButton))]
    public class AbilityView : MonoBehaviour, IUserInterfaceView<DisciplinePower>
    {
        [SerializeField, Required]
        private TooltipButton _button;
        [SerializeField, Required]
        private TextMeshProUGUI _text;
        private Action _cachedAction;

        public static event Action OnAbilityPick;

        public void UpdateView(IGameDataProvider<DisciplinePower> dataProvider)
        {
            if (dataProvider == null)
                throw new ArgumentNullException("Null data provider in AbilityView!");
            gameObject.SetActive(true);
            _button.SetContent(dataProvider.Data.Tooltip);
            _button.image.sprite = dataProvider.Data.Icon;
            _button.interactable = !CombatManager.CurrentActiveActor.SpellbookManager.IsPowerOnCooldown(dataProvider.Data);
            _text.text = dataProvider.Data.PowerName;
        }

        public void SetupAction(Action onClik)
        {
            _cachedAction = onClik;
        }

        public void InvokeCachedAction()
        {
            OnAbilityPick?.Invoke();
            ActionBarUI.instance.SetBarAction((int)BarAction.SELECT_TARGET);
            _cachedAction?.Invoke();
        }

        public void SetupTooltip(GameTooltip tooltipInstance)
        {
            _button.Initialize(tooltipInstance);
        }
    }
}
