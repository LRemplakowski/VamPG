using NaughtyAttributes;
using Redcode.Awaiting;
using SunsetSystems.Resources;
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
    public class DialogueWithHistoryView : DialogueViewBase, IPortraitUpdateReciever
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
        [Space, Header("Portrait")]
        [SerializeField]
        private GameObject _photoParent;
        [SerializeField]
        private Image _photo;
        [SerializeField]
        private TextMeshProUGUI _photoText;

        private StringBuilder _stringBuilder = new();
        private int _cachedMaxVisibleCharacters = default;
        private int _lineHistoryTextLength = default;

        private const string ROLL_SUCCESS_TAG = "success";
        private const string ROLL_FAIL_TAG = "failure";

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

        public async override void InterruptLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
        {
            _cancellationTokenSource.Cancel();
            await new WaitForUpdate();
            _lineHistory.maxVisibleCharacters = _lineHistory.text.Length;
            _lineHistoryTextLength = _lineHistory.text.Length;
            _cachedMaxVisibleCharacters = _lineHistory.text.Length;
            onDialogueLineFinished?.Invoke();
        }

        public async override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
        {
            _clampScrollbarNextFrame = true;
            _lineHistory.maxVisibleCharacters = _cachedMaxVisibleCharacters;
            int stringBuilderTextLengthPreAppend = _stringBuilder.Length;
            _stringBuilder
                .AppendLine("")
                .Append("<size=26>");
            AppendRollPrefix(dialogueLine);
            _stringBuilder
                .AppendLine($"<color=\"red\">{dialogueLine.CharacterName}:</size></color>")
                .AppendLine(dialogueLine.TextWithoutCharacterName.Text);
            _lineHistoryTextLength = _stringBuilder.Length;
            _lineHistory.text = _stringBuilder.ToString();
            LayoutRebuilder.MarkLayoutForRebuild(_lineHistory.transform.parent as RectTransform);
            if (_typewriterEffect)
            {
                _cancellationTokenSource = new();
                _cachedTypewriteTask = Task.Run(TypewriteLineText, _cancellationTokenSource.Token);
                await _cachedTypewriteTask;
            }
            if (_cachedTypewriteTask != null && _cachedTypewriteTask.IsCanceled)
                return;
            _cachedTypewriteTask = null;
            _lineHistory.maxVisibleCharacters = _lineHistory.text.Length;
            await new WaitForSeconds(_lineCompletionDelay);
            onDialogueLineFinished?.Invoke();

            async void TypewriteLineText()
            {
                await new WaitForUpdate();
                if (_typeSpeed <= 0)
                    return;
                float accumulator = 0f;
                float secondsPerLetter = 1 / _typeSpeed;

                while (_cachedMaxVisibleCharacters < _lineHistoryTextLength)
                {
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

        private void AppendRollPrefix(LocalizedLine dialogueLine)
        {
            if (dialogueLine.Metadata == null || dialogueLine.Metadata.Length <= 0)
                return;
            if (dialogueLine.Metadata.Contains(ROLL_SUCCESS_TAG))
                _stringBuilder.Append("(Success) ");
            else if (dialogueLine.Metadata.Contains(ROLL_FAIL_TAG))
                _stringBuilder.Append("(Failure) ");
        }

        public void InitializeSpeakerPhoto(string speakerID)
        {
            Sprite sprite = this.GetSpeakerPortrait(speakerID);
            if (sprite != null)
            {
                _photo.sprite = this.GetSpeakerPortrait(speakerID);
                _photoText.text = speakerID;
                _photoParent.SetActive(true);
            }
            else
            {
                _photoParent.SetActive(false);
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

    public interface IPortraitUpdateReciever
    {
        void InitializeSpeakerPhoto(string speakerID);
    }
}
