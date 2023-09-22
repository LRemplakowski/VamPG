using SunsetSystems.Party;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace SunsetSystems
{
    public class CharacterSelector : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _selectedCharacterText;
        private static string _selectedCharacterKey = "";

        [SerializeField]
        private UnityEvent OnSelectedCharacterChanged;
        public static string SelectedCharacterKey 
        { 
            get
            {
                if (string.IsNullOrEmpty(_selectedCharacterKey))
                    _selectedCharacterKey = PartyManager.Instance.MainCharacter.ID;
                return _selectedCharacterKey;
            }
        }

        private void OnEnable()
        {
            UpdateSelectedText();
        }

        private void UpdateSelectedText()
        {
            _selectedCharacterText.text = PartyManager.Instance.GetPartyMemberByID(SelectedCharacterKey).Name;
        }

        public void NextCharacter()
        {
            int currentIndex = PartyManager.Instance.AllCoterieMembers.FindIndex(cd => cd.DatabaseID.Equals(SelectedCharacterKey));
            currentIndex = currentIndex + 1 < PartyManager.Instance.AllCoterieMembers.Count ? currentIndex + 1 : 0;
            _selectedCharacterKey = PartyManager.Instance.AllCoterieMembers[currentIndex].DatabaseID;
            OnSelectedCharacterChanged?.Invoke();
            UpdateSelectedText();
        }

        public void PreviousCharacter()
        {
            int currentIndex = PartyManager.Instance.AllCoterieMembers.FindIndex(cd => cd.DatabaseID.Equals(SelectedCharacterKey));
            currentIndex = currentIndex - 1 >= 0 ? currentIndex - 1 : PartyManager.Instance.AllCoterieMembers.Count - 1;
            _selectedCharacterKey = PartyManager.Instance.AllCoterieMembers[currentIndex].DatabaseID;
            OnSelectedCharacterChanged?.Invoke();
            UpdateSelectedText();
        }
    }
}
