using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DD;
using SunsetSystems.Management;

namespace SunsetSystems.Dialogue
{
    public class DialogueManager : Manager
    {
        [SerializeField]
        private VerticalLayoutGroup _historyContent, _optionsContent;
        [SerializeField]
        private GameObject _dialogueLine, _dialogueChoice;
        [SerializeField]
        private DialogueWindowUI _dialogueWindow;

        private List<GameObject> _choices = new List<GameObject>();
        private List<GameObject> _history = new List<GameObject>();

        // The dialogue to play
        // Drop the conversation file asset in here using the Inspector
        [SerializeField, Tooltip("The dialogue to play")]
        private TextAsset _dialogueFile;

        // The conversation, parsed and loaded at runtime
        private DD.Dialogue _loadedDialogue;

        // A player that can track progress through the dialogue
        private DialoguePlayer _dialoguePlayer;

        public delegate void OnDialogueBegin();
        public static OnDialogueBegin onDialogueBegin;
        public delegate void OnDialogueEnd();
        public static OnDialogueEnd onDialogueEnd;

        #region Enable&Disable
        private void OnEnable()
        {
            // set up your unique code to display dialogues
            DialoguePlayer.GlobalOnShowMessage += OnShowMessage;
            DialoguePlayer.GlobalOnEvaluateCondition += OnEvaluateCondition;
            DialoguePlayer.GlobalOnExecuteScript += OnExecuteScript;

            // if you want to handle a particular dialogue differently, you can use these instead
            //m_dialoguePlayer.OverrideOnShowMessage += OnShowMessageSpecial;
            //m_dialoguePlayer.OverrideOnEvaluateCondition += OnEvaluateConditionSpecial;
            //m_dialoguePlayer.OverrideOnExecuteScript += OnExecuteScriptSpecial;
        }

        private void OnDisable()
        {
            // set up your unique code to display dialogues
            DialoguePlayer.GlobalOnShowMessage -= OnShowMessage;
            DialoguePlayer.GlobalOnEvaluateCondition -= OnEvaluateCondition;
            DialoguePlayer.GlobalOnExecuteScript -= OnExecuteScript;

            // if you want to handle a particular dialogue differently, you can use these instead
            //m_dialoguePlayer.OverrideOnShowMessage += OnShowMessageSpecial;
            //m_dialoguePlayer.OverrideOnEvaluateCondition += OnEvaluateConditionSpecial;
            //m_dialoguePlayer.OverrideOnExecuteScript += OnExecuteScriptSpecial;
        }
        #endregion

        private void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (_dialogueWindow == null)
            {
                _dialogueWindow = FindObjectOfType<DialogueWindowUI>(true);
                _historyContent = _dialogueWindow.GetComponentInChildren<DialogueHistory>(true).GetComponent<VerticalLayoutGroup>();
                _optionsContent = _dialogueWindow.GetComponentInChildren<DialogueOptions>(true).GetComponent<VerticalLayoutGroup>();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (_dialoguePlayer != null)
                _dialoguePlayer.Update();
        }

        public void StartDialogue(TextAsset dialogueFile)
        {
            Debug.Log("Starting dialogue");
            // load the dialogue
            _loadedDialogue = DD.Dialogue.FromAsset(dialogueFile);
            // create a player to play through the dialogue
            _dialoguePlayer = new DialoguePlayer(_loadedDialogue);

            _dialoguePlayer.OnDialogueEnded += OnDialogueEnded;
            StateManager.SetCurrentState(GameState.Conversation);
            _dialoguePlayer.Play();
            OnDialogueStarted(_dialoguePlayer);
        }

        private void OnDialogueStarted(DialoguePlayer sender)
        {
            onDialogueBegin?.Invoke();
        }

        private void OnDialogueEnded(DialoguePlayer sender)
        {
            ClearChoices();
            ClearHistory();
            onDialogueEnd?.Invoke();
            _dialoguePlayer.OnDialogueEnded -= OnDialogueEnded;
            _dialoguePlayer = null;
            _dialogueWindow.gameObject.SetActive(false);
            StateManager.SetCurrentState(GameState.Exploration);
        }

        private void OnExecuteScript(DialoguePlayer sender, string script)
        {
            DialogueExecution.Execute(script);
        }

        private bool OnEvaluateCondition(DialoguePlayer sender, string script)
        {
            return DialogueCondition.Evaluate(script);
        }

        private void OnShowMessage(DialoguePlayer sender, ShowMessageNode node)
        {
            ClearChoices();
            Debug.LogWarning("nodeCharacter: " + node.Character);
            AddNewLine(node.Character, node.GetText(GameManager.GetLanguage()));
            ShowMessageNodeChoice choiceNode = node as ShowMessageNodeChoice;
            if (choiceNode)
            {
                AddNodeOptions(choiceNode);
            }
            else
            {
                _dialoguePlayer.AdvanceMessage(0);
            }
        }

        public void SelectChoice(int choiceID)
        {
            _dialoguePlayer.AdvanceMessage(choiceID);
        }

        private void AddNewLine(string author, string message)
        {
            TextMeshProUGUI t = Instantiate(_dialogueLine, _historyContent.transform).GetComponent<TextMeshProUGUI>();
            t.text = author + ": " + message;
            _history.Add(t.gameObject);
        }

        private void AddNewChoice(string text, int id)
        {
            GameObject choiceLine = Instantiate(_dialogueChoice, _optionsContent.transform);
            choiceLine.GetComponent<TextMeshProUGUI>().text = text;
            choiceLine.GetComponent<DialogueChoiceButton>().choiceID = id;
            _choices.Add(choiceLine);
        }
        private void AddNewChoice(string text, int id, bool enabled)
        {
            GameObject choiceLine = Instantiate(_dialogueChoice, _optionsContent.transform);
            choiceLine.GetComponent<TextMeshProUGUI>().text = text;
            choiceLine.GetComponent<DialogueChoiceButton>().choiceID = id;
            choiceLine.GetComponent<Button>().interactable = enabled;
            _choices.Add(choiceLine);
        }

        private void AddNodeOptions(ShowMessageNodeChoice choiceNode)
        {
            if (choiceNode != null)
            {
                for (int i = 0; i < choiceNode.Choices.Length; i++)
                {
                    Choice c = choiceNode.Choices[i];
                    if (c.IsCondition)
                        AddNewChoice(choiceNode.GetChoiceText(i, GameManager.GetLanguage()), i, DialogueCondition.Evaluate(c.Condition));
                    else
                        AddNewChoice(choiceNode.GetChoiceText(i, GameManager.GetLanguage()), i);
                }
            }
        }

        public void ClearChoices()
        {
            foreach (GameObject c in _choices)
            {
                Destroy(c);
            }
            _choices.Clear();
        }

        public void ClearHistory()
        {
            foreach (GameObject line in _history)
            {
                Destroy(line);
            }
            _history.Clear();
        }
    }
}
