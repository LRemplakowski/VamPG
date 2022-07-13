using SunsetSystems.Dialogue;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SunsetSystems.Dialogue
{
    [RequireComponent(typeof(Button)), RequireComponent(typeof(TextMeshProUGUI))]
    public class DialogueChoiceButton : ExposableMonobehaviour
    {
        private DialogueManager _manager;
        [ReadOnly]
        public int choiceID;

        // Start is called before the first frame update
        void Start()
        {
            _manager = DialogueManager.Instance;
        }

        public void OnClick()
        {
            Debug.Log("Choice selected: " + GetComponent<TextMeshProUGUI>().text + "\nChoice ID: " + choiceID);
            if (GetComponent<Button>().interactable)
                _manager.SelectChoice(choiceID);
        }

        private void OnDestroy()
        {
            Debug.LogWarning("Destroying choice: " + GetComponent<TextMeshProUGUI>().text);
        }
    }
}
