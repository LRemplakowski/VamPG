using NaughtyAttributes;
using Redcode.Awaiting;
using SunsetSystems.Audio;
using SunsetSystems.Party;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        [SerializeField]
        private List<OptionView> _optionViews;

        private StringBuilder _lineHistoryStringBuilder = new();

        private const string ROLL_SUCCESS_TAG = "success";
        private const string ROLL_FAIL_TAG = "failure";
        private const string ALWAYS_SHOW_OPTION = "showAlways";

        private Action<int> OnOptionSelected;

        [SerializeField]
        private UnityEvent OnDialogueStarted, OnDialogueFinished;

        private bool _clampScrollbarNextFrame;
        private bool RequestedLineInterrupt { get; set; } = false;

        private void Awake()
        {
            _optionViews = new();
        }

        private void Start()
        {
            DialogueManager.RegisterView(this);
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (_clampScrollbarNextFrame)
                ClampScrollbar();
            _clampScrollbarNextFrame = false;
        }

        public override void DialogueComplete()
        {
            OnDialogueFinished?.Invoke();
        }

        public override void DialogueStarted()
        {
            _lineHistory.text = string.Empty;
            _lineHistory.maxVisibleCharacters = 0;
            _lineHistoryStringBuilder = new();
            OnDialogueStarted?.Invoke();
        }

        public override void DismissLine(Action onDismissalComplete)
        {
            onDismissalComplete?.Invoke();
        }

        public async override void InterruptLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
        {
            _clampScrollbarNextFrame = true;
            AudioManager.Instance.StopSFXPlayback();
            AudioManager.Instance.PlayTypewriterEnd();
            RequestedLineInterrupt = true;
            await new WaitForUpdate();
            _clampScrollbarNextFrame = true;
            _lineHistory.maxVisibleCharacters = _lineHistory.textInfo.characterCount;
            onDialogueLineFinished?.Invoke();
        }

        public async override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
        {
            RequestedLineInterrupt = false;
            _clampScrollbarNextFrame = true;
            UpdateSpeakerPhoto(dialogueLine.CharacterName);
            _lineHistory.maxVisibleCharacters = _lineHistory.textInfo.characterCount;
            string formattedLineText = BuildFormattedText(dialogueLine);
            _lineHistory.text = formattedLineText;
            LayoutRebuilder.MarkLayoutForRebuild(_lineHistory.transform.parent as RectTransform);
            if (_typewriterEffect)
            {
                AudioManager.Instance.PlayTyperwriterLoop();
                await TypewriteText(dialogueLine);
                if (RequestedLineInterrupt)
                    return;
            }
            AudioManager.Instance.PlayTypewriterEnd();
            _clampScrollbarNextFrame = true;
            await new WaitForSeconds(_lineCompletionDelay);
            onDialogueLineFinished?.Invoke();
        }

        private async Task TypewriteText(LocalizedLine line)
        {
            await new WaitForUpdate();
            if (_typeSpeed <= 0)
                return;
            _lineHistory.maxVisibleCharacters += line.CharacterName?.Length ?? 0;
            float _currentVisibleCharacters = _lineHistory.maxVisibleCharacters;
            while (_lineHistory.textInfo.characterCount > _lineHistory.maxVisibleCharacters)
            {
                await new WaitForUpdate();
                if (RequestedLineInterrupt)
                    break;
                _lineHistory.maxVisibleCharacters = Mathf.RoundToInt(_currentVisibleCharacters);
                _currentVisibleCharacters += Time.deltaTime * _typeSpeed;
            }
            if (RequestedLineInterrupt)
            {
                _clampScrollbarNextFrame = true;
                _lineHistory.maxVisibleCharacters = _lineHistory.textInfo.characterCount;
                return;
            }
        }

        private string BuildFormattedText(LocalizedLine line)
        {
            _lineHistoryStringBuilder
                .AppendLine("")
                .Append("<size=26>");
            AppendRollPrefix(line);
            if (line.CharacterName != null && string.IsNullOrWhiteSpace(line.CharacterName) == false)
            {
                _lineHistoryStringBuilder
                    .Append($"<color=\"red\">{line.CharacterName}:</color>");
            }
            _lineHistoryStringBuilder
                .AppendLine("</size>")
                .AppendLine(line.TextWithoutCharacterName.Text);
            return _lineHistoryStringBuilder.ToString();
        }

        private void AppendRollPrefix(LocalizedLine dialogueLine)
        {
            if (dialogueLine.Metadata == null || dialogueLine.Metadata.Length <= 0)
                return;
            if (dialogueLine.Metadata.Contains(ROLL_SUCCESS_TAG))
                _lineHistoryStringBuilder.Append("(Success) ");
            else if (dialogueLine.Metadata.Contains(ROLL_FAIL_TAG))
                _lineHistoryStringBuilder.Append("(Failure) ");
        }

        private void UpdateSpeakerPhoto(string characterName)
        {
            string speakerID;
            if (characterName == null || string.IsNullOrWhiteSpace(characterName))
            {
                speakerID = PartyManager.MainCharacter.Data.ID;
                characterName = PartyManager.MainCharacter.Data.FullName;
            }
            else
            {
                if (DialogueHelper.VariableStorage.TryGetValue(characterName, out speakerID) == false)
                {
                    speakerID = PartyManager.MainCharacter.Data.ID;
                    characterName = PartyManager.MainCharacter.Data.FullName;
                }
            }
            Sprite sprite = this.GetSpeakerPortrait(speakerID);
            if (sprite != null)
            {
                _photo.sprite = sprite;
                _photoText.text = characterName;
                _photoParent.SetActive(true);
            }
            else
            {
                _photoParent.SetActive(false);
                Debug.LogError($"Cannot find portrait for creature with ID {speakerID} and no placeholder portrait found!");
            }
        }

        public void ClampScrollbar()
        {
            _scrollbar.value = 0f;
        }

        public override void RunOptions(DialogueOption[] dialogueOptions, Action<int> onOptionSelected)
        {
            _clampScrollbarNextFrame = true;
            for (int i = 0; i < dialogueOptions.Length; i++)
            {
                DialogueOption option = dialogueOptions[i];
                if (option is null)
                {
                    Debug.LogError($"Encountered null option!");
                    return;
                }
                bool alwaysShowOption = option.Line.Metadata?.Contains(ALWAYS_SHOW_OPTION) ?? false;
                if (option.IsAvailable == false && _showUnavailableOptions == false && alwaysShowOption == false)
                {
                    continue;
                }
                OptionView optionView = Instantiate(_optionViewPrefab, _optionParent);
                optionView.transform.SetAsLastSibling();
                optionView.OnOptionSelected = OptionViewWasSelected;
                _optionViews.Add(optionView);
                optionView.Option = option;
                optionView.interactable = option.IsAvailable;
                optionView.gameObject.SetActive(true);
            }

            OnOptionSelected = onOptionSelected;

            async void OptionViewWasSelected(DialogueOption option)
            {
                _clampScrollbarNextFrame = true;
                UpdateSpeakerPhoto(option.Line.CharacterName);
                AudioManager.Instance.PlayTypewriterEnd();
                await new WaitForUpdate();
                _clampScrollbarNextFrame = true;
                string formattedLineText = BuildFormattedText(option.Line);
                _lineHistory.text = formattedLineText;
                await new WaitForUpdate();
                _clampScrollbarNextFrame = true;
                _lineHistory.maxVisibleCharacters = _lineHistory.textInfo.characterCount;
                OnOptionSelected(option.DialogueOptionID);
                CleanupOptions();
            }

            void CleanupOptions()
            {
                _optionViews.ForEach(ov => Destroy(ov.gameObject));
                _optionViews.Clear();
            }
        }

        public override void UserRequestedViewAdvancement()
        {
            _clampScrollbarNextFrame = true;
            AudioManager.Instance.PlayTypewriterEnd();
            RequestedLineInterrupt = true;
            requestInterrupt?.Invoke();
        }
    }

    public interface IPortraitUpdateReciever
    {
        void InitializeSpeakerPhoto(string speakerID);
    }
}
