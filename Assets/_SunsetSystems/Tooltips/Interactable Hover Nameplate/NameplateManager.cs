using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using SunsetSystems.Entities;
using SunsetSystems.Entities.Interactable;
using UnityEngine.InputSystem;

namespace SunsetSystems.Tooltips
{
    public class NameplateManager : AbstractTooltipManager<HoverNameplateData, HoverNameplate>
    {
        [TabGroup("Nameplate Manager")]
        [ShowInInspector, ReadOnly]
        private Dictionary<IHoverNameplateSource, HoverNameplateData> _nameplateSourceDataMap = new();
        [TabGroup("Nameplate Manager")]
        [ShowInInspector, ReadOnly]
        private List<IHoverNameplateSource> _alwaysActiveNameplateSources = new();

        protected void Awake()
        {
            _nameplateSourceDataMap = new();
        }

        private void OnEnable()
        {
            IHoverNameplateSource.OnHoverStatusChange += HandleNameplateHover;
        }

        private void OnDisable()
        {
            IHoverNameplateSource.OnHoverStatusChange -= HandleNameplateHover;
        }

        public void HandleNameplateHover(IHoverNameplateSource nameplateReciever, bool visible)
        {
            if (visible)
                ShowNewNameplate(nameplateReciever);
            else
                HideNameplate(nameplateReciever);
        }

        private void ShowNewNameplate(IHoverNameplateSource nameplateSource)
        {
            if (_nameplateSourceDataMap.ContainsKey(nameplateSource))
                return;
            var nameplateData = new HoverNameplateData(nameplateSource);
            ShowTooltip(nameplateData);
            _nameplateSourceDataMap[nameplateSource] = nameplateData;
        }

        private void HideNameplate(IHoverNameplateSource nameplateSource)
        {
            if (_nameplateSourceDataMap.TryGetValue(nameplateSource, out var data) && _alwaysActiveNameplateSources.Contains(nameplateSource) is false)
            {
                HideTooltip(data);
                _nameplateSourceDataMap.Remove(nameplateSource);
            }
        }

        public void OnHighlightInteractables(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _alwaysActiveNameplateSources.AddRange(InteractableEntity.InteractablesInScene.Select(interactable => interactable as IHoverNameplateSource));
                _alwaysActiveNameplateSources.ForEach(active => HandleNameplateHover(active, true));
            }
            else if (context.canceled)
            {
                var previouslyActiveCache = new List<IHoverNameplateSource>(_alwaysActiveNameplateSources);
                _alwaysActiveNameplateSources.Clear();
                previouslyActiveCache.ForEach(active => HandleNameplateHover(active, false));
            }
        }
    }
}
