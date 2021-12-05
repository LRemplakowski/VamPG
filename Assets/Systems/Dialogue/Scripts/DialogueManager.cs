using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DD;
using System;
using Utils.Singleton;
using SunsetSystems.Management;

public class DialogueManager : Manager
{
    [SerializeField]
    private VerticalLayoutGroup historyContent, optionsContent;
    [SerializeField]
    private GameObject dialogueLine, dialogueChoice;
    [SerializeField]
    private DialogueWindowUI dialogueWindow;

    private List<GameObject> choices = new List<GameObject>();
    private List<GameObject> history = new List<GameObject>();

    // The dialogue to play
    // Drop the conversation file asset in here using the Inspector
    public TextAsset DialogueFile;

    // The conversation, parsed and loaded at runtime
    private Dialogue m_loadedDialogue;

    // A player that can track progress through the dialogue
    private DialoguePlayer m_dialoguePlayer;

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
        if (dialogueWindow == null)
        {
            dialogueWindow = FindObjectOfType<DialogueWindowUI>(true);
            historyContent = dialogueWindow.GetComponentInChildren<DialogueHistory>(true).GetComponent<VerticalLayoutGroup>();
            optionsContent = dialogueWindow.GetComponentInChildren<DialogueOptions>(true).GetComponent<VerticalLayoutGroup>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_dialoguePlayer != null)
            m_dialoguePlayer.Update();
    }

    public void StartDialogue(TextAsset dialogueFile)
    {
        dialogueWindow.gameObject.SetActive(true);
        // load the dialogue
        m_loadedDialogue = Dialogue.FromAsset(dialogueFile);
        // create a player to play through the dialogue
        m_dialoguePlayer = new DialoguePlayer(m_loadedDialogue);

        m_dialoguePlayer.OnDialogueEnded += OnDialogueEnded;
        StateManager.SetCurrentState(GameState.Conversation);
        m_dialoguePlayer.Play();
        OnDialogueStarted(m_dialoguePlayer);
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
        m_dialoguePlayer.OnDialogueEnded -= OnDialogueEnded;
        m_dialoguePlayer = null;
        dialogueWindow.gameObject.SetActive(false);
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
            m_dialoguePlayer.AdvanceMessage(0);
        }
    }

    public void SelectChoice(int choiceID)
    {
        m_dialoguePlayer.AdvanceMessage(choiceID);
    }

    private void AddNewLine(string author, string message)
    {
        TextMeshProUGUI t = Instantiate(dialogueLine, historyContent.transform).GetComponent<TextMeshProUGUI>();
        t.text = author + ": " + message;
        history.Add(t.gameObject);
    }

    private void AddNewChoice(string text, int id)
    {
        GameObject choiceLine = Instantiate(dialogueChoice, optionsContent.transform);
        choiceLine.GetComponent<TextMeshProUGUI>().text = text;
        choiceLine.GetComponent<DialogueChoiceButton>().choiceID = id;
        choices.Add(choiceLine);
    }
    private void AddNewChoice(string text, int id, bool enabled)
    {
        GameObject choiceLine = Instantiate(dialogueChoice, optionsContent.transform);
        choiceLine.GetComponent<TextMeshProUGUI>().text = text;
        choiceLine.GetComponent<DialogueChoiceButton>().choiceID = id;
        choiceLine.GetComponent<Button>().interactable = enabled;
        choices.Add(choiceLine);
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
        foreach (GameObject c in choices)
        {
            Destroy(c);
        }
        choices.Clear();
    }

    public void ClearHistory()
    {
        foreach (GameObject line in history)
        {
            Destroy(line);
        }
        history.Clear();
    }
}
