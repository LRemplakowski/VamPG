using NaughtyAttributes;
using Redcode.Awaiting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Yarn.Unity;

namespace SunsetSystems.Dialogue
{
    public class DialogueWithHistoryView : DialogueViewBase
    {
        [SerializeField, Required]
        private TextMeshProUGUI _lineHistory;
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
        [SerializeField]
        private float _lineCompletionDelay = .5f;

        private StringBuilder _stringBuilder = new();
        private int _cachedMaxVisibleCharacters = default;
        private int _lineHistoryTextLength = default;

        private const string LINE_INDENT_OPEN = "<line-indent=-20>";
        private const string LINE_INDENT_CLOSE = "</line-indent>";

        private CancellationTokenSource _cancellationTokenSource;
        private Task _cachedTypewriteTask;

        private Action<int> OnOptionSelected;

        [SerializeField]
        private UnityEvent OnDialogueStarted, OnDialogueFinished;

        private bool _clampScrollbarNextFrame;

        private void Awake()
        {
            _optionViews = new();
        }

        private void Start()
        {
            DialogueManager.RegisterView(this);
            gameObject.SetActive(false);
        }

        public override void DialogueComplete()
        {
            OnDialogueFinished?.Invoke();
        }

        public override void DialogueStarted()
        {
            _lineHistoryTextLength = 0;
            _cachedMaxVisibleCharacters = 0;
            _lineHistory.text = string.Empty;
            _lineHistory.maxVisibleCharacters = _cachedMaxVisibleCharacters;
            _stringBuilder = new();

            foreach (OptionView optionView in _optionViews)
            {
                optionView?.gameObject.SetActive(false);
            }
            OnDialogueStarted?.Invoke();
        }

        public override void DismissLine(Action onDismissalComplete)
        {
            onDismissalComplete?.Invoke();
        }

        public override void InterruptLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
        {
            _cancellationTokenSource.Cancel();
            _lineHistory.maxVisibleCharacters = _lineHistory.text.Length;
            onDialogueLineFinished?.Invoke();
        }

        public async override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
        {
            _clampScrollbarNextFrame = true;
            _lineHistoryTextLength += dialogueLine.Text.Text.Length;
            _lineHistory.maxVisibleCharacters = _cachedMaxVisibleCharacters;
            _stringBuilder
                .AppendLine("")
                //.Append(LINE_INDENT_OPEN)
                .Append(dialogueLine.Text.Text);
                //.Append(LINE_INDENT_CLOSE);
            _lineHistory.text = _stringBuilder.ToString();
            Debug.Break();
            LayoutRebuilder.MarkLayoutForRebuild(_lineHistory.transform.parent as RectTransform);
            if (_typewriterEffect)
            {
                _cancellationTokenSource = new();
                _cachedTypewriteTask = TypewriteLineText(_cancellationTokenSource.Token);
                await _cachedTypewriteTask;
            }
            if (_cachedTypewriteTask != null && _cachedTypewriteTask.IsCanceled)
                return;
            _cachedTypewriteTask = null;
            _lineHistory.maxVisibleCharacters = _lineHistory.text.Length;
            await new WaitForSeconds(_lineCompletionDelay);
            onDialogueLineFinished?.Invoke();

            async Task TypewriteLineText(CancellationToken token)
            {
                await new WaitForUpdate();
                if (_typeSpeed <= 0)
                    return;
                float accumulator = 0f;
                float secondsPerLetter = 1 / _typeSpeed;

                while (_cachedMaxVisibleCharacters < _lineHistoryTextLength)
                {
                    if (token.IsCancellationRequested)
                        return;
                    await new WaitForUpdate();
                    _lineHistory.maxVisibleCharacters = _cachedMaxVisibleCharacters;
                    accumulator += Time.deltaTime;

                    while (accumulator >= secondsPerLetter)
                    {
                        _cachedMaxVisibleCharacters += 1;
                        accumulator -= secondsPerLetter;
                    }
                }
            }
        }

        public void CheckForClampScrollbar()
        {
            if (_clampScrollbarNextFrame)
                _scrollbar.SetValueWithoutNotify(0f);
            _clampScrollbarNextFrame = false;
        }

        public override void RunOptions(DialogueOption[] dialogueOptions, Action<int> onOptionSelected)
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
                CleanupOptions();
                OnOptionSelected(option.DialogueOptionID);
            }

            void CleanupOptions()
            {
                _optionViews.ForEach(ov => ov.gameObject.SetActive(false));
            }
        }

        public override void UserRequestedViewAdvancement()
        {
            _cancellationTokenSource.Cancel();
            requestInterrupt?.Invoke();
        }
    }
}
