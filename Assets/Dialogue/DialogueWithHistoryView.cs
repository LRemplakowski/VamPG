using NaughtyAttributes;
using Redcode.Awaiting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

namespace SunsetSystems.Dialogue
{
    public class DialogueWithHistoryView : DialogueViewBase
    {
        [SerializeField, Required]
        private TextMeshProUGUI _lineText, _lineHistory;
        [SerializeField, Required]
        private Scrollbar _scrollbar;
        [SerializeField, Required]
        private Transform _optionParent;
        [SerializeField, Required]
        private OptionView _optionViewPrefab;
        private List<OptionView> _optionViews;
        [SerializeField]
        private bool _typewriterEffect;
        [SerializeField, ShowIf("_typewriterEffect"), MinValue(0f)]
        private float _typeSpeed;
        [SerializeField]
        private bool _showUnavailableOptions;

        private StringBuilder _stringBuilder = new();

        private const string LINE_INDENT_OPEN = "<line-indent=-20>";
        private const string LINE_INDENT_CLOSE = "</line-indent>";

        private Action<int> OnOptionSelected;

        private void Awake()
        {
            _optionViews = new();
        }

        public override void DialogueComplete()
        {

        }

        public override void DialogueStarted()
        {
            _lineHistory.text = string.Empty;
            _lineText.text = string.Empty;

            foreach (OptionView optionView in _optionViews)
            {
                optionView?.gameObject.SetActive(false);
            }
        }

        public async override void DismissLine(Action onDismissalComplete)
        {
            _stringBuilder.Clear();
            _stringBuilder.Append(_lineHistory.text).Append("\n").Append(_lineText.text);
            _lineHistory.text = _stringBuilder.ToString();
            _lineText.text = string.Empty;
            await Task.Run(async () =>
            {
                await new WaitForUpdate();
                float lerpTime = 0f;
                while (_scrollbar.value < 1f)
                {
                    lerpTime += Time.deltaTime;
                    _scrollbar.value = Mathf.Lerp(1f, 0f, lerpTime);
                    await new WaitForUpdate();
                }
            });
            onDismissalComplete?.Invoke();
        }

        public override void InterruptLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
        {
            _stringBuilder.Clear();
            _stringBuilder.Append(LINE_INDENT_OPEN)
                .Append(dialogueLine.Text.Text)
                .Append(LINE_INDENT_CLOSE);
            _lineText.text = _stringBuilder.ToString();
            _lineText.maxVisibleCharacters = _lineText.text.Length;
            onDialogueLineFinished?.Invoke();
        }

        public async override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
        {
            _stringBuilder.Clear();
            _stringBuilder.Append(LINE_INDENT_OPEN)
                .Append(dialogueLine.Text.Text)
                .Append(LINE_INDENT_CLOSE);
            _lineText.text = _stringBuilder.ToString();
            if (_typewriterEffect)
                await TypewriteLineText();
            onDialogueLineFinished?.Invoke();

            async Task TypewriteLineText()
            {
                if (_typeSpeed <= 0)
                    return;

                _lineText.maxVisibleCharacters = 0;
                float accumulator = 0f;
                float secondsPerLetter = 1 / _typeSpeed;

                while (_lineText.maxVisibleCharacters < _lineText.text.Length)
                {
                    await new WaitForUpdate();
                    accumulator += Time.deltaTime;

                    while (accumulator >= secondsPerLetter)
                    {
                        _lineText.maxVisibleCharacters += 1;
                        accumulator -= secondsPerLetter;
                    }
                }

                _lineText.maxVisibleCharacters = _lineText.text.Length;
            }
        }

        public async override void RunOptions(DialogueOption[] dialogueOptions, Action<int> onOptionSelected)
        {
            foreach (OptionView optionView in _optionViews)
            {
                optionView.gameObject.SetActive(false);
            }

            while (dialogueOptions.Length > _optionViews.Count)
            {
                OptionView optionView = CreateNewOptionView();
                optionView.gameObject.SetActive(false);
            }

            int optionViewsCreated = 0;

            for (int i = 0; i < dialogueOptions.Length; i++)
            {
                OptionView optionView = _optionViews[i];
                DialogueOption option = dialogueOptions[i];
                if (option is null)
                    return;

                if (option.IsAvailable == false && _showUnavailableOptions == false)
                {
                    continue;
                }
                optionView.Option = option;
                optionView.interactable = option.IsAvailable;
                optionView.gameObject.SetActive(true);

                if (optionViewsCreated == 0)
                {
                    optionView.Select();
                }

                optionViewsCreated += 1;
            }

            await Task.Run(async () =>
            {
                await new WaitForUpdate();
                float lerpTime = 0f;
                while (_scrollbar.value < 1f)
                {
                    lerpTime += Time.deltaTime;
                    _scrollbar.value = Mathf.Lerp(1f, 0f, lerpTime);
                    await new WaitForUpdate();
                }
            });

            OnOptionSelected = onOptionSelected;

            OptionView CreateNewOptionView()
            {
                OptionView optionView = Instantiate(_optionViewPrefab, _optionParent);
                optionView.transform.SetAsLastSibling();

                optionView.OnOptionSelected = OptionViewWasSelected;
                _optionViews.Add(optionView);

                return optionView;
            }

            void OptionViewWasSelected(DialogueOption option)
            {
                OnOptionSelected(option.DialogueOptionID);
                CleanupOptions();
            }

            void CleanupOptions()
            {
                _optionViews.ForEach(ov => ov.gameObject.SetActive(false));
            }
        }

        public override void UserRequestedViewAdvancement()
        {
            requestInterrupt?.Invoke();
        }
    }
}
