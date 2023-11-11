using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UltEvents;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    [Serializable]
    public class CompositeButton : Selectable, IPointerClickHandler, IEventSystemHandler, ISubmitHandler
    {
        [SerializeField]
        private List<ButtonCompositeData> compositeData;
        [SerializeField]
        private UltEvent OnClick = new();

        private void Press()
        {
            if (IsActive() && IsInteractable())
            {
                UISystemProfilerApi.AddMarker("Button.onClick", this);
                OnClick.Invoke();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                Press();
            }
        }

        public void OnSubmit(BaseEventData eventData)
        {
            Press();
            if (IsActive() && IsInteractable())
            {
                DoStateTransition(SelectionState.Pressed, instant: false);
                StartCoroutine(OnFinishSubmit());
            }
        }

        private IEnumerator OnFinishSubmit()
        {
            float fadeTime = base.colors.fadeDuration;
            float elapsedTime = 0f;
            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            DoStateTransition(base.currentSelectionState, instant: false);
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            if (!gameObject.activeInHierarchy)
                return;
            base.DoStateTransition(state, instant);
            foreach (ButtonCompositeData data in compositeData)
            {
                switch (data.Transition)
                {
                    case Transition.None:
                        continue;
                    case Transition.ColorTint:
                        DoStateColorTint(state, data.ColorTransition, instant);
                        break;
                    case Transition.SpriteSwap:
                        DoStateSpriteSwap(state, data.SpriteTransition);
                        break;
                    case Transition.Animation:
                        DoStateAnimation(state, data.AnimationTransition);
                        break;
                }
            }
        }

        private void DoStateColorTint(SelectionState state, CustomColorBlock colorTransition, bool instant)
        {
            Color color = Color.magenta;
            switch (state)
            {
                case SelectionState.Normal:
                    color = colorTransition.NormalColor;
                    break;
                case SelectionState.Highlighted:
                    color = colorTransition.HighlightedColor;
                    break;
                case SelectionState.Pressed:
                    color = colorTransition.PressedColor;
                    break;
                case SelectionState.Selected:
                    color = colorTransition.SelectedColor;
                    break;
                case SelectionState.Disabled:
                    color = colorTransition.DisabledColor;
                    break;
            }
            StartColorTween(colorTransition.Image, color * colorTransition.ColorMultiplier, colorTransition.FadeDuration, instant);
        }

        private void DoStateSpriteSwap(SelectionState state, CustomSpriteState spriteTransition)
        {
            Sprite sprite = null;
            switch (state)
            {
                case SelectionState.Normal:
                    break;
                case SelectionState.Highlighted:
                    sprite = spriteTransition.HighlightedSprite;
                    break;
                case SelectionState.Pressed:
                    sprite = spriteTransition.PressedSprite;
                    break;
                case SelectionState.Selected:
                    sprite = spriteTransition.SelectedSprite;
                    break;
                case SelectionState.Disabled:
                    sprite = spriteTransition.DisabledSprite;
                    break;
            }
            DoSpriteSwap(spriteTransition.Image, sprite);
        }

        private void DoStateAnimation(SelectionState state, CustomAnimationTriggers animationTransition)
        {
            int triggerHash = default;
            switch (state)
            {
                case SelectionState.Normal:
                    triggerHash = animationTransition.NormalTriggerHash;
                    break;
                case SelectionState.Highlighted:
                    triggerHash = animationTransition.HighlightedTriggerHash;
                    break;
                case SelectionState.Pressed:
                    triggerHash = animationTransition.NormalTriggerHash;
                    break;
                case SelectionState.Selected:
                    triggerHash = animationTransition.SelectedTriggerHash;
                    break;
                case SelectionState.Disabled:
                    triggerHash = animationTransition.DisabledTriggerHash;
                    break;
            }
            TriggerAnimation(animationTransition, triggerHash);
        }

        private void StartColorTween(Image targetGraphic, Color targetColor, float fadeDuration, bool instant)
        {
            if (!(targetGraphic == null))
            {
                targetGraphic.CrossFadeColor(targetColor, instant ? 0f : fadeDuration, ignoreTimeScale: true, useAlpha: true);
            }
        }

        private void DoSpriteSwap(Image image, Sprite newSprite)
        {
            if (!(image == null))
            {
                image.overrideSprite = newSprite;
            }
        }

        private void TriggerAnimation(CustomAnimationTriggers animationTriggers, int triggerHash)
        {
            if (animationTriggers.Animator != null && animationTriggers.Animator.isActiveAndEnabled && animationTriggers.Animator.hasBoundPlayables)
            {
                animationTriggers.Animator.ResetTrigger(animationTriggers.NormalTriggerHash);
                animationTriggers.Animator.ResetTrigger(animationTriggers.HighlightedTriggerHash);
                animationTriggers.Animator.ResetTrigger(animationTriggers.PressedTriggerHash);
                animationTriggers.Animator.ResetTrigger(animationTriggers.SelectedTriggerHash);
                animationTriggers.Animator.ResetTrigger(animationTriggers.DisabledTriggerHash);
                animationTriggers.Animator.SetTrigger(triggerHash);
            }
        }
    }

    [Serializable]
    public struct ButtonCompositeData
    {
        public Selectable.Transition Transition;
        public CustomColorBlock ColorTransition;
        public CustomSpriteState SpriteTransition;
        public CustomAnimationTriggers AnimationTransition;
    }

    [Serializable]
    public struct CustomColorBlock : IEquatable<CustomColorBlock>
    {
        [field: SerializeField]
        public Image Image { get; private set; }
        [field:SerializeField]
        public Color NormalColor { get; private set; }
        [field: SerializeField]
        public Color HighlightedColor { get; private set; }
        [field: SerializeField]
        public Color PressedColor { get; private set; }
        [field: SerializeField]
        public Color SelectedColor { get; private set; }
        [field: SerializeField]
        public Color DisabledColor { get; private set; }

        [field: SerializeField]
        public float ColorMultiplier { get; private set; }
        [field: SerializeField]
        public float FadeDuration { get; private set; }

        public override bool Equals(object obj) => obj is CustomColorBlock block && Equals(block);

        public bool Equals(CustomColorBlock other)
        {
            return Image == other.Image &&
                NormalColor == other.NormalColor &&
                HighlightedColor == other.HighlightedColor &&
                PressedColor == other.PressedColor &&
                SelectedColor == other.SelectedColor &&
                DisabledColor == other.DisabledColor &&
                ColorMultiplier == other.ColorMultiplier &&
                FadeDuration == other.FadeDuration;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    [Serializable]
    public struct CustomSpriteState : IEquatable<CustomSpriteState>
    {
        [field: SerializeField]
        public Image Image { get; private set; }
        [field: SerializeField]
        public Sprite HighlightedSprite { get; private set; }
        [field: SerializeField]
        public Sprite PressedSprite { get; private set; }
        [field: SerializeField]
        public Sprite SelectedSprite { get; private set; }
        [field: SerializeField]
        public Sprite DisabledSprite { get; private set; }

        public bool Equals(CustomSpriteState other)
        {
            return HighlightedSprite == other.HighlightedSprite && PressedSprite == other.PressedSprite && SelectedSprite == other.SelectedSprite && DisabledSprite == other.DisabledSprite;
        }
    }

    [Serializable]
    public class CustomAnimationTriggers
    {
        [field: SerializeField]
        public Animator Animator { get; private set; }
        [SerializeField]
        private string _normalTrigger = "Normal";
        [SerializeField]
        private string _highlightedTrigger = "Highlighted";
        [SerializeField]
        private string _pressedTrigger = "Pressed";
        [SerializeField]
        private string _selectedTrigger = "Selected";
        [SerializeField]
        private string _disabledTrigger = "Disabled";

        public int NormalTriggerHash => Animator.StringToHash(_normalTrigger);
        public int HighlightedTriggerHash => Animator.StringToHash(_highlightedTrigger);
        public int PressedTriggerHash => Animator.StringToHash(_pressedTrigger);
        public int SelectedTriggerHash => Animator.StringToHash(_selectedTrigger);
        public int DisabledTriggerHash => Animator.StringToHash(_disabledTrigger);
    }
}
