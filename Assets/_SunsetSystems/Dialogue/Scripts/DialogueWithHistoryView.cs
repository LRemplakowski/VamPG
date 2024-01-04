using Sirenix.OdinInspector;
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

        public event Action OnOptionsPresented, OnOptionSelectedCustom;
        private Action<int> OnOptionSelected;

        [SerializeField]
        private UnityEvent OnDialogueStarted, OnDialogueFinished;

        private bool _clampScrollbarNextFrame;
        private bool _requestedLineInterrupt = false;
        private bool _optionsPresented = false;

        public void Cleanup()
        {
            _lineHistory.text = string.Empty;
            _lineHistory.maxVisibleCharacters = 0;
            _lineHistoryStringBuilder = new();
        }

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
            {
                ClampScrollbar();
                _clampScrollbarNextFrame = false;
            }
        }

        public override void DialogueComplete()
        {
            OnDialogueFinished?.Invoke();
        }

        public override void DialogueStarted()
        {
            _lineHistory.maxVisibleCharacters = 0;
            _clampScrollbarNextFrame = true;
            _lineHistory.text = string.Empty;
            _lineHistoryStringBuilder = new();
            Cleanup();
            OnDialogueStarted?.Invoke();
        }

        public override void DismissLine(Action onDismissalComplete)
        {
            _clampScrollbarNextFrame = true;
            onDismissalComplete?.Invoke();
        }

        public async override void InterruptLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
        {
            _clampScrollbarNextFrame = true;
            AudioManager.Instance.PlayTypewriterEnd();
            await new WaitForUpdate();
            _clampScrollbarNextFrame = true;
            _lineHistory.maxVisibleCharacters = _lineHistory.textInfo.characterCount;
            onDialogueLineFinished?.Invoke();
        }

        public async override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
        {
            _requestedLineInterrupt = false;
            _clampScrollbarNextFrame = true;
            UpdateSpeakerPhoto(dialogueLine.CharacterName);
            _lineHistory.maxVisibleCharacters = _lineHistory.textInfo.characterCount;
            await new WaitForUpdate();
            string formattedLineText = BuildFormattedText(dialogueLine);
            _lineHistory.text = formattedLineText;
            LayoutRebuilder.MarkLayoutForRebuild(_lineHistory.transform.parent as RectTransform);
            if (_typewriterEffect && _typeSpeed > 0)
            {
                AudioManager.Instance.PlayTyperwriterLoop();
                await TypewriteText(dialogueLine);
                if (_requestedLineInterrupt)
                    return;
            }
            await new WaitForUpdate();
            _lineHistory.maxVisibleCharacters = _lineHistory.textInfo.characterCount;
            AudioManager.Instance.PlayTypewriterEnd();
            _clampScrollbarNextFrame = true;
            await new WaitForSecondsRealtime(_lineCompletionDelay);
            onDialogueLineFinished?.Invoke();
        }

        private async Task TypewriteText(LocalizedLine line)
        {
            _lineHistory.maxVisibleCharacters += line.CharacterName?.Length ?? 0;
            float _currentVisibleCharacters = _lineHistory.maxVisibleCharacters;
            while (_lineHistory.textInfo.characterCount > _lineHistory.maxVisibleCharacters)
            {
                await new WaitForUpdate();
                if (_requestedLineInterrupt)
                {
                    _clampScrollbarNextFrame = true;
                    break;
                }
                _lineHistory.maxVisibleCharacters = Mathf.RoundToInt(_currentVisibleCharacters);
                _currentVisibleCharacters += Time.deltaTime * _typeSpeed;
            }
        }

        private string BuildFormattedText(LocalizedLine line)
        {
            _lineHistoryStringBuilder
                .AppendLine("");
            bool appended = AppendRollPrefix(line);
            if (line.CharacterName != null && string.IsNullOrWhiteSpace(line.CharacterName) == false)
            {
                if (appended == false)
                    _lineHistoryStringBuilder.Append("<size=26>");
                appended = true;
                _lineHistoryStringBuilder
                    .Append($"<color=\"red\">{line.CharacterName}:</color>");
            }
            if (appended)
                _lineHistoryStringBuilder.AppendLine("</size>");
            _lineHistoryStringBuilder
                .AppendLine(line.TextWithoutCharacterName.Text);
            return _lineHistoryStringBuilder.ToString();
        }

        private bool AppendRollPrefix(LocalizedLine dialogueLine)
        {
            if (dialogueLine.Metadata == null || dialogueLine.Metadata.Length <= 0)
                return false;
            _lineHistoryStringBuilder.Append("<size=26>");
            if (dialogueLine.Metadata.Contains(ROLL_SUCCESS_TAG))
                _lineHistoryStringBuilder.Append("(Success) ");
            else if (dialogueLine.Metadata.Contains(ROLL_FAIL_TAG))
                _lineHistoryStringBuilder.Append("(Failure) ");
            return true;
        }

        private async void UpdateSpeakerPhoto(string characterName)
        {
            string speakerID;
            if (characterName == null || string.IsNullOrWhiteSpace(characterName))
            {
                speakerID = PartyManager.Instance.MainCharacter.References.CreatureData.ReadableID;
                characterName = PartyManager.Instance.MainCharacter.Name;
            }
            else
            {
                if (DialogueHelper.VariableStorage.TryGetValue(characterName, out speakerID) == false)
                {
                    speakerID = PartyManager.Instance.MainCharacter.References.CreatureData.ReadableID;
                    characterName = PartyManager.Instance.MainCharacter.Name;
                }
            }
            Sprite sprite = await this.GetSpeakerPortrait(speakerID);
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
            _optionsPresented = true;
            OnOptionsPresented?.Invoke();
            OnOptionSelected = onOptionSelected;

            async void OptionViewWasSelected(DialogueOption option)
            {
                CleanupOptions();
                if (option.Line.CharacterName != null)
                {
                    string formattedLineText = BuildFormattedText(option.Line);
                    _lineHistory.text = formattedLineText;
                    await new WaitForUpdate();
                    _lineHistory.maxVisibleCharacters = _lineHistory.textInfo.characterCount;
                    _clampScrollbarNextFrame = true;
                }
                OnOptionSelected(option.DialogueOptionID);
                OnOptionSelectedCustom?.Invoke();
                _optionsPresented = false;
                _clampScrollbarNextFrame = true;
            }

            void CleanupOptions()
            {
                _optionViews.ForEach(ov => Destroy(ov.gameObject));
                _optionViews.Clear();
            }
        }

        public override void UserRequestedViewAdvancement()
        {
            if (_requestedLineInterrupt || _optionsPresented)
                return;
            _clampScrollbarNextFrame = true;
            AudioManager.Instance.PlayTypewriterEnd();
            _requestedLineInterrupt = true;
            requestInterrupt?.Invoke();
        }


        public void SetTypeWriterSpeed(float speed)
        {
            _typeSpeed = speed;
        }
    }

    public interface IPortraitUpdateReciever
    {
        void InitializeSpeakerPhoto(string speakerID);
    }
}
