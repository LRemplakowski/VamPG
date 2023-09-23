using SunsetSystems.Combat;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Entities.Interfaces;
using SunsetSystems.Spellbook;
using SunsetSystems.UI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SunsetSystems.UI
{
    public class ActiveAbilitiesDisplay : MonoBehaviour, IUserInterfaceUpdateReciever<DisciplinePower>
    {
        [SerializeField]
        private Transform _viewsParent;
        [SerializeField]
        private AbilityView _viewPrefab;
        [SerializeField]
        private GameTooltip _tooltipInstance;

        private readonly List<AbilityView> _viewsPool = new();

        private void OnEnable()
        {
            AbilityView.OnAbilityPick += DisableDisplay;
        }

        private void OnDisable()
        {
            AbilityView.OnAbilityPick -= DisableDisplay;
        }

        private void DisableDisplay()
        {
            DisableViews();
            gameObject.SetActive(false);
        }

        public void DisplayAbilities(List<IGameDataProvider<DisciplinePower>> data, Transform hookPoint)
        {
            if (data == null || data.Count <= 0)
            {
                Debug.LogWarning("ActiveAbilitiesDisplay recieved an empty or null collection!");
                return;
            }
            transform.SetParent(hookPoint, false);
            DisableViews();
            UpdateViews(data);
            gameObject.SetActive(true);
        }

        public void DisableViews()
        {
            _viewsPool.ForEach(v => v.gameObject.SetActive(false));
        }

        public void UpdateViews(List<IGameDataProvider<DisciplinePower>> data)
        {
            IMagicUser spellcastingActor = CombatManager.Instance.CurrentActiveActor.MagicUser;
            foreach (IGameDataProvider<DisciplinePower> ability in data)
            {
                AbilityView view = GetViewFromPool();
                view.SetupAction(Spellcaster.GetPowerAction(ability.Data, spellcastingActor));
                view.UpdateView(ability);
            }
        }

        private AbilityView GetViewFromPool()
        {
            AbilityView result = _viewsPool.FirstOrDefault(v => v.gameObject.activeSelf == false);
            if (result == null)
            {
                result = Instantiate(_viewPrefab, _viewsParent);
                result.gameObject.SetActive(false);
                result.SetupTooltip(_tooltipInstance);
                _viewsPool.Add(result);
            }
            return result;
        }
    }
}
